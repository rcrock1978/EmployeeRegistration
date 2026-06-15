import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getMemberById } from "../lib/api";
import type { MemberDetail } from "../types/api";

export default function AdminMemberDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [member, setMember] = useState<MemberDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    setLoading(true);
    setError(null);
    getMemberById(id)
      .then((res) => {
        if (res.isSuccess && res.value) {
          setMember(res.value);
        } else {
          setError("Member not found.");
        }
      })
      .catch(() => setError("Network error."))
      .finally(() => setLoading(false));
  }, [id]);

  if (loading) return <p className="text-gray-500">Loading...</p>;
  if (error) return <p className="text-red-700">{error}</p>;
  if (!member) return <p className="text-gray-500">Member not found.</p>;

  return (
    <div>
      <button onClick={() => navigate("/admin/members")} className="text-blue-600 hover:underline mb-4 block">
        &larr; Back to list
      </button>

      <h1 className="text-2xl font-bold text-gray-800 mb-6">
        {member.personName.firstName} {member.personName.lastName}
      </h1>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <section className="border rounded p-4">
          <h2 className="font-semibold mb-3">Personal Info</h2>
          <DetailRow label="Title" value={member.personName.title} />
          <DetailRow label="First Name" value={member.personName.firstName} />
          <DetailRow label="Middle Name" value={member.personName.middleName} />
          <DetailRow label="Last Name" value={member.personName.lastName} />
          <DetailRow label="Suffix" value={member.personName.suffix} />
          <DetailRow label="Alias" value={member.personName.alias} />
          <DetailRow label="Date of Birth" value={member.demographics.dateOfBirth} />
          <DetailRow label="Place of Birth" value={member.demographics.placeOfBirth} />
          <DetailRow label="Nationality" value={member.demographics.nationality} />
          <DetailRow label="Gender" value={member.demographics.gender} />
          <DetailRow label="Civil Status" value={member.demographics.civilStatus} />
          <DetailRow label="Education" value={member.demographics.highestEducationalAttainment} />
        </section>

        <section className="border rounded p-4">
          <h2 className="font-semibold mb-3">Contact</h2>
          <DetailRow label="Email" value={member.contactDetails.emailAddress} />
          <DetailRow label="Phone" value={member.contactDetails.contactNumber} />
          <DetailRow label="Dependents" value={String(member.numberOfDependents)} />
        </section>

        <section className="border rounded p-4">
          <h2 className="font-semibold mb-3">Family</h2>
          {member.relatedPersons.spouse && (
            <DetailRow label="Spouse" value={`${member.relatedPersons.spouse.firstName} ${member.relatedPersons.spouse.lastName}`} />
          )}
          <DetailRow label="Mother" value={member.relatedPersons.motherMaidenName} />
          {member.relatedPersons.father && (
            <DetailRow label="Father" value={`${member.relatedPersons.father.firstName} ${member.relatedPersons.father.lastName}`} />
          )}
        </section>

        <section className="border rounded p-4">
          <h2 className="font-semibold mb-3">Government IDs</h2>
          <DetailRow label="TIN" value={member.governmentIds.tin} />
          <DetailRow label="SSS" value={member.governmentIds.sss} />
          <DetailRow label="Primary ID Type" value={member.primaryId.type} />
          <DetailRow label="Primary ID Number" value={member.primaryId.number} />
          <DetailRow label="Issue Date" value={member.primaryId.issueDate} />
          <DetailRow label="Expiry Date" value={member.primaryId.expiryDate} />
        </section>

        <section className="border rounded p-4">
          <h2 className="font-semibold mb-3">Current Address</h2>
          <DetailRow label="Street" value={member.currentAddress.streetNameAndNumber} />
          <DetailRow label="City" value={member.currentAddress.city} />
          <DetailRow label="Postal Code" value={member.currentAddress.postalCode} />
          <DetailRow label="Barangay" value={member.currentAddress.barangay} />
          <DetailRow label="Province" value={member.currentAddress.province} />
          <DetailRow label="Country" value={member.currentAddress.country} />
        </section>

        <section className="border rounded p-4">
          <h2 className="font-semibold mb-3">Permanent Address</h2>
          {member.permanentAddress.sameAsCurrent ? (
            <p className="text-sm text-gray-500">Same as current address</p>
          ) : member.permanentAddress.address ? (
            <>
              <DetailRow label="Street" value={member.permanentAddress.address.streetNameAndNumber} />
              <DetailRow label="City" value={member.permanentAddress.address.city} />
              <DetailRow label="Postal Code" value={member.permanentAddress.address.postalCode} />
              <DetailRow label="Barangay" value={member.permanentAddress.address.barangay} />
              <DetailRow label="Province" value={member.permanentAddress.address.province} />
              <DetailRow label="Country" value={member.permanentAddress.address.country} />
            </>
          ) : (
            <p className="text-sm text-gray-500">Not provided</p>
          )}
        </section>

        <section className="border rounded p-4">
          <h2 className="font-semibold mb-3">Employment</h2>
          <DetailRow label="Level" value={member.employment.employeeLevel} />
          <DetailRow label="Company" value={member.employment.companyTradeName} />
          <DetailRow label="Company ID" value={member.employment.companyIdNumber} />
          <DetailRow label="Income" value={String(member.employment.grossIncome)} />
          <DetailRow label="Income Period" value={member.employment.incomePeriod} />
          <DetailRow label="Occupation" value={member.employment.occupation} />
          <DetailRow label="Hired From" value={member.employment.hiredFrom} />
          <DetailRow label="Hired To" value={member.employment.hiredTo} />
        </section>

        <section className="border rounded p-4">
          <h2 className="font-semibold mb-3">Emergency Contact</h2>
          <DetailRow label="Name" value={member.emergencyContact.contactName} />
          <DetailRow label="Relationship" value={member.emergencyContact.relationship} />
          <DetailRow label="Phone" value={member.emergencyContact.contactNumber} />
        </section>

        <section className="border rounded p-4">
          <h2 className="font-semibold mb-3">Status & Consent</h2>
          <DetailRow label="Status" value={member.status} />
          <DetailRow label="Consent Given" value={member.consent.consentGiven ? "Yes" : "No"} />
          <DetailRow label="Attestation" value={member.consent.attestation ? "Yes" : "No"} />
          <DetailRow label="Signed By" value={member.consent.signatureName} />
          <DetailRow label="Created" value={new Date(member.createdOn).toLocaleString()} />
          {member.updatedOn && <DetailRow label="Updated" value={new Date(member.updatedOn).toLocaleString()} />}
        </section>
      </div>
    </div>
  );
}

function DetailRow({ label, value }: { label: string; value?: string | null }) {
  if (!value) return null;
  return (
    <div className="flex justify-between py-1">
      <span className="text-sm text-gray-500">{label}</span>
      <span className="text-sm font-medium">{value}</span>
    </div>
  );
}
