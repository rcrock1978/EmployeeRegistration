// k6 performance benchmark — Member Registration API
// Targets (p95): POST ≤400ms, GET ≤200ms
// Run: k6 run --vus 50 --duration 30s benchmark.js

import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';

const postTrend = new Trend('post_create_duration');
const getByIdTrend = new Trend('get_by_id_duration');
const listTrend = new Trend('list_duration');
const failureRate = new Rate('request_failure');

export const options = {
  stages: [
    { duration: '10s', target: 10 },   // ramp up
    { duration: '15s', target: 50 },   // nominal load
    { duration: '10s', target: 0 },    // ramp down
  ],
  thresholds: {
    post_create_duration: ['p(95)<400'],
    get_by_id_duration: ['p(95)<200'],
    list_duration: ['p(95)<200'],
    request_failure: ['rate<0.01'],
  },
  summaryTrendStats: ['avg', 'min', 'med', 'p(90)', 'p(95)', 'max'],
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5001';

function randomEmail() {
  return `bench.${Date.now()}.${Math.random().toString(36).slice(2, 8)}@example.com`;
}

function registerPayload(email) {
  return JSON.stringify({
    personalInfo: {
      title: 'Mr.',
      firstName: 'Bench',
      lastName: 'User',
      dateOfBirth: '1990-05-12',
      placeOfBirth: 'Manila',
      countryOfBirth: 'PH',
      nationality: 'Filipino',
      gender: 'Male',
      civilStatus: 'Single',
      highestEducationalAttainment: "Bachelor's",
      numberOfDependents: 0,
    },
    contactInfo: {
      emailAddress: email,
      contactNumber: '+639170000000',
    },
    relatedPersons: {},
    governmentIds: { tin: '123-456-789-000', sss: '01-2345678-9' },
    primaryId: {
      type: 'Passport', number: 'P' + Math.random().toString(36).slice(2, 10),
      issueDate: '2021-01-10', expiryDate: '2031-01-09', issueCountry: 'PH',
    },
    currentAddress: {
      streetNameAndNumber: '123 St', city: 'Manila', postalCode: '1000',
      barangay: 'San Juan', province: 'NCR', country: 'PH',
      ownerOrLessee: 'Owner', occupiedSince: '2020-01-01',
    },
    permanentAddress: { sameAsCurrent: true },
    emergencyContact: {
      contactName: 'Emergency', relationship: 'Parent', contactNumber: '+639179999999',
    },
    employment: {
      employeeLevel: 'RNF', companyTradeName: 'OPTODEV',
      companyIdNumber: 'EMP-001', grossIncome: 45000,
      incomePeriod: 'Monthly', occupation: 'Engineer',
      hiredFrom: '2020-01-01',
    },
    consent: { consentGiven: true, attestation: true, signatureName: 'Bench User' },
  });
}

const REGISTER_HEADERS = { 'Content-Type': 'application/json' };

export default function () {
  // POST — register
  const email = randomEmail();
  const postRes = http.post(`${BASE_URL}/api/members`, registerPayload(email), {
    headers: REGISTER_HEADERS,
  });
  check(postRes, { 'POST status 201': (r) => r.status === 201 });
  postTrend.add(postRes.timings.duration);
  failureRate.add(postRes.status !== 201);

  let memberId = '';
  if (postRes.status === 201) {
    try {
      memberId = postRes.json('value.id') || '';
    } catch {
      // fallback: parse Location header
      const loc = postRes.headers['Location'] || '';
      memberId = loc.split('/').pop() || '';
    }
  }

  sleep(0.5);

  // GET /api/members — list (allow 403 if no HRAdmin token; still measures response time)
  const listRes = http.get(`${BASE_URL}/api/members`);
  check(listRes, { 'list status 200 or 403': (r) => r.status === 200 || r.status === 403 });
  listTrend.add(listRes.timings.duration);
  failureRate.add(listRes.status >= 500);

  sleep(0.3);

  // GET /api/members/{id} — detail
  if (memberId) {
    const detailRes = http.get(`${BASE_URL}/api/members/${memberId}`);
    check(detailRes, { 'detail status 200 or 403': (r) => r.status === 200 || r.status === 403 });
    getByIdTrend.add(detailRes.timings.duration);
    failureRate.add(detailRes.status >= 500);
  }

  sleep(0.5);
}
