import { useState } from "react";
import { useRegistration } from "../hooks/useRegistration";
import { registerMember } from "../lib/api";
import StepIndicator from "./StepIndicator";
import PersonalInfoStep from "./steps/PersonalInfoStep";
import FamilyStep from "./steps/FamilyStep";
import GovernmentIdsStep from "./steps/GovernmentIdsStep";
import ResidencyStep from "./steps/ResidencyStep";
import EmploymentConsentStep from "./steps/EmploymentConsentStep";

export default function RegistrationWizard() {
  const {
    state,
    submitting,
    submitError,
    submittedId,
    setSubmitting,
    setSubmitError,
    setSubmittedId,
    goToStep,
    saveStepData,
    reset,
  } = useRegistration();

  const [completedSteps, setCompletedSteps] = useState<Set<number>>(new Set());

  function handleStepComplete(step: number, data: any) {
    saveStepData(step, data);
    setCompletedSteps((prev) => new Set(prev).add(step));

    if (step < 4) {
      goToStep(step + 1);
    }
  }

  async function handleSubmit(data: any) {
    saveStepData(4, data);

    const payload = buildPayload();
    if (!payload) return;

    setSubmitting(true);
    setSubmitError(null);

    try {
      const res = await registerMember(payload);
      if (res.ok) {
        const body = await res.json();
        setSubmittedId(body.value?.id ?? null);
      } else if (res.status === 409) {
        const body = await res.json();
        const fieldErrors = body.error?.details ?? [];
        const emailErr = fieldErrors.find(
          (f: any) => f.field === "contactInfo.emailAddress"
        );
        setSubmitError(
          emailErr?.message ?? "This email is already registered."
        );
      } else if (res.status === 400) {
        const body = await res.json();
        const details = body.error?.details ?? [];
        setSubmitError(
          `Validation failed: ${details.map((d: any) => d.message).join("; ")}`
        );
        goToStep(0);
      } else {
        setSubmitError("An unexpected error occurred. Please try again.");
      }
    } catch {
      setSubmitError("Network error. Please check your connection and try again.");
    } finally {
      setSubmitting(false);
    }
  }

  function buildPayload() {
    const p = state.personalInfo;
    const f = state.family;
    const g = state.governmentIds;
    const r = state.residency;
    const e = state.employmentConsent;

    return {
      personalInfo: {
        title: p.title,
        firstName: p.firstName,
        middleName: p.middleName || null,
        lastName: p.lastName,
        suffix: p.suffix || null,
        alias: p.alias || null,
        dateOfBirth: p.dateOfBirth,
        placeOfBirth: p.placeOfBirth,
        countryOfBirth: p.countryOfBirth,
        nationality: p.nationality,
        gender: p.gender,
        civilStatus: p.civilStatus,
        religion: p.religion || null,
        highestEducationalAttainment: p.highestEducationalAttainment,
        numberOfDependents: Number(p.numberOfDependents),
      },
      contactInfo: {
        emailAddress: (p as any).email ?? "",
        contactNumber: (p as any).contactNumber ?? "",
      },
      relatedPersons: {
        spouse: f.spouse ?? null,
        motherMaidenName: f.motherMaidenName || null,
        father: f.father ?? null,
      },
      governmentIds: {
        tin: g.tin,
        sss: g.sss,
      },
      primaryId: {
        type: (g as any).primaryId?.type,
        number: (g as any).primaryId?.number,
        issueDate: (g as any).primaryId?.issueDate,
        expiryDate: (g as any).primaryId?.expiryDate,
        issueCountry: (g as any).primaryId?.issueCountry,
      },
      currentAddress: {
        streetNameAndNumber: r.streetNameAndNumber,
        city: r.city,
        postalCode: r.postalCode,
        barangay: r.barangay,
        subdivisionPurok: r.subdivisionPurok || null,
        province: r.province,
        country: r.country,
        ownerOrLessee: r.ownerOrLessee,
        occupiedSince: r.occupiedSince,
      },
      permanentAddress: r.sameAsCurrent
        ? { sameAsCurrent: true }
        : {
            sameAsCurrent: false,
            address: {
              streetNameAndNumber: r.permStreetNameAndNumber,
              city: r.permCity,
              postalCode: r.permPostalCode,
              barangay: r.permBarangay,
              subdivisionPurok: r.permSubdivisionPurok || null,
              province: r.permProvince,
              country: r.permCountry,
              ownerOrLessee: r.permOwnerOrLessee,
              occupiedSince: r.permOccupiedSince,
            },
          },
      emergencyContact: {
        contactName: e.contactName,
        relationship: e.relationship,
        contactNumber: e.contactNumber,
      },
      employment: {
        employeeLevel: e.employeeLevel,
        companyTradeName: e.companyTradeName,
        companyIdNumber: e.companyIdNumber,
        grossIncome: Number(e.grossIncome),
        incomePeriod: e.incomePeriod,
        occupation: e.occupation,
        hiredFrom: e.hiredFrom,
        hiredTo: e.hiredTo || null,
      },
      consent: {
        consentGiven: e.consentGiven === true,
        attestation: e.attestation === true,
        signatureName: e.signatureName,
      },
    };
  }

  if (submittedId) {
    return (
      <div className="max-w-2xl mx-auto p-8 text-center">
        <div className="text-6xl mb-4">✓</div>
        <h2 className="text-2xl font-bold mb-2">Registration Complete</h2>
        <p className="text-gray-600 mb-6">
          Your registration has been submitted successfully.
        </p>
        <p className="text-sm text-gray-500 mb-6">
          Reference ID: <span className="font-mono font-bold">{submittedId}</span>
        </p>
        <button
          onClick={reset}
          className="bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700"
        >
          Register Another
        </button>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto p-4">
      <h1 className="text-2xl font-bold text-center mb-2">
        OPTODEV Membership Registration
      </h1>
      <p className="text-sm text-gray-500 text-center mb-6">
        Fill out all steps to complete your membership application
      </p>

      <StepIndicator currentStep={state.currentStep} completedSteps={completedSteps} />

      {submitError && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-6">
          {submitError}
        </div>
      )}

      {submitting && (
        <div className="text-center py-8">
          <div className="animate-spin w-8 h-8 border-4 border-blue-600 border-t-transparent rounded-full mx-auto mb-4" />
          <p className="text-gray-600">Submitting your registration...</p>
        </div>
      )}

      {!submitting && state.currentStep === 0 && (
        <PersonalInfoStep
          initial={state.personalInfo}
          onNext={(data) => handleStepComplete(0, data)}
        />
      )}

      {!submitting && state.currentStep === 1 && (
        <FamilyStep
          initial={state.family}
          civilStatus={(state.personalInfo.civilStatus as string) ?? ""}
          onNext={(data) => handleStepComplete(1, data)}
          onBack={() => goToStep(0)}
        />
      )}

      {!submitting && state.currentStep === 2 && (
        <GovernmentIdsStep
          initial={state.governmentIds}
          onNext={(data) => handleStepComplete(2, data)}
          onBack={() => goToStep(1)}
        />
      )}

      {!submitting && state.currentStep === 3 && (
        <ResidencyStep
          initial={state.residency}
          onNext={(data) => handleStepComplete(3, data)}
          onBack={() => goToStep(2)}
        />
      )}

      {!submitting && state.currentStep === 4 && (
        <EmploymentConsentStep
          initial={state.employmentConsent}
          onSubmit={handleSubmit}
          onBack={() => goToStep(3)}
        />
      )}
    </div>
  );
}
