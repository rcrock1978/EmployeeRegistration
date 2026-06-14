import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { employmentConsentSchema, type EmploymentConsentData } from "../../lib/schemas";

interface Props {
  initial: Partial<EmploymentConsentData>;
  onSubmit: (data: EmploymentConsentData) => void;
  onBack: () => void;
}

const EMPLOYEE_LEVELS = ["RNF", "NCE", "RSE", "SSE", "TL", "AM", "MGR"];

export default function EmploymentConsentStep({ initial, onSubmit, onBack }: Props) {
  const {
    register,
    handleSubmit,
    formState: { errors, isValid },
  } = useForm<EmploymentConsentData>({
    mode: "onBlur",
    resolver: zodResolver(employmentConsentSchema),
    defaultValues: {
      employeeLevel: "",
      companyTradeName: "",
      companyIdNumber: "",
      grossIncome: 0,
      incomePeriod: "",
      occupation: "",
      hiredFrom: "",
      hiredTo: "",
      contactName: "",
      relationship: "",
      contactNumber: "",
      consentGiven: false,
      attestation: false,
      signatureName: "",
      ...initial,
    },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <fieldset className="border rounded p-4">
        <legend className="font-semibold px-2">Employment Details</legend>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium">Employee Level *</label>
            <select {...register("employeeLevel")} className="w-full border rounded p-2">
              <option value="">Select</option>
              {EMPLOYEE_LEVELS.map((l) => <option key={l}>{l}</option>)}
            </select>
            {errors.employeeLevel && <p className="text-red-500 text-xs">{errors.employeeLevel.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium">Company Trade Name *</label>
            <input {...register("companyTradeName")} className="w-full border rounded p-2" />
            {errors.companyTradeName && <p className="text-red-500 text-xs">{errors.companyTradeName.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium">Company ID Number *</label>
            <input {...register("companyIdNumber")} className="w-full border rounded p-2" />
            {errors.companyIdNumber && <p className="text-red-500 text-xs">{errors.companyIdNumber.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium">Gross Income *</label>
            <input type="number" {...register("grossIncome", { valueAsNumber: true })} className="w-full border rounded p-2" />
            {errors.grossIncome && <p className="text-red-500 text-xs">{errors.grossIncome.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium">Income Period *</label>
            <select {...register("incomePeriod")} className="w-full border rounded p-2">
              <option value="">Select</option>
              <option>Daily</option>
              <option>Weekly</option>
              <option>Semi-Monthly</option>
              <option>Monthly</option>
              <option>Annually</option>
            </select>
            {errors.incomePeriod && <p className="text-red-500 text-xs">{errors.incomePeriod.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium">Occupation *</label>
            <input {...register("occupation")} className="w-full border rounded p-2" />
            {errors.occupation && <p className="text-red-500 text-xs">{errors.occupation.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium">Hired From *</label>
            <input type="date" {...register("hiredFrom")} className="w-full border rounded p-2" />
            {errors.hiredFrom && <p className="text-red-500 text-xs">{errors.hiredFrom.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium">Hired To</label>
            <input type="date" {...register("hiredTo")} className="w-full border rounded p-2" />
          </div>
        </div>
      </fieldset>

      <fieldset className="border rounded p-4">
        <legend className="font-semibold px-2">Emergency Contact</legend>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label className="block text-sm font-medium">Contact Name *</label>
            <input {...register("contactName")} className="w-full border rounded p-2" />
            {errors.contactName && <p className="text-red-500 text-xs">{errors.contactName.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium">Relationship *</label>
            <input {...register("relationship")} className="w-full border rounded p-2" />
            {errors.relationship && <p className="text-red-500 text-xs">{errors.relationship.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium">Contact Number *</label>
            <input
              {...register("contactNumber")}
              type="tel"
              className="w-full border rounded p-2"
            />
            {errors.contactNumber && <p className="text-red-500 text-xs">{errors.contactNumber.message}</p>}
          </div>
        </div>
      </fieldset>

      <fieldset className="border rounded p-4">
        <legend className="font-semibold px-2">Privacy Consent & Attestation</legend>
        <div className="space-y-4">
          <label className="flex items-start gap-2 cursor-pointer">
            <input type="checkbox" {...register("consentGiven")} className="mt-1 w-5 h-5" />
            <span className="text-sm">
              I consent to the collection and processing of my personal information in accordance with the Data Privacy Act of 2012 (RA 10173).
            </span>
          </label>
          {errors.consentGiven && <p className="text-red-500 text-xs">{errors.consentGiven.message}</p>}

          <label className="flex items-start gap-2 cursor-pointer">
            <input type="checkbox" {...register("attestation")} className="mt-1 w-5 h-5" />
            <span className="text-sm">
              I attest that all information provided is true, complete, and accurate to the best of my knowledge.
            </span>
          </label>
          {errors.attestation && <p className="text-red-500 text-xs">{errors.attestation.message}</p>}

          <div>
            <label className="block text-sm font-medium">Full Name (as signature) *</label>
            <input {...register("signatureName")} className="w-full border rounded p-2" />
            {errors.signatureName && <p className="text-red-500 text-xs">{errors.signatureName.message}</p>}
          </div>
        </div>
      </fieldset>

      <div className="flex justify-between">
        <button type="button" onClick={onBack} className="bg-gray-300 px-6 py-2 rounded hover:bg-gray-400">
          Back
        </button>
        <button
          type="submit"
          className="bg-green-600 text-white px-8 py-2 rounded hover:bg-green-700 disabled:opacity-50"
          disabled={!isValid}
        >
          Submit
        </button>
      </div>
    </form>
  );
}
