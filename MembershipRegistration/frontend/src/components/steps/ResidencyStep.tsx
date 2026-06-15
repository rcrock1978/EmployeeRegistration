import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { residencySchema, type ResidencyData } from "../../lib/schemas";

interface Props {
  initial: Partial<ResidencyData>;
  onNext: (data: ResidencyData) => void;
  onBack: () => void;
}

export default function ResidencyStep({ initial, onNext, onBack }: Props) {
  const [sameAsCurrent, setSameAsCurrent] = useState(
    initial.sameAsCurrent ?? true
  );

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<ResidencyData>({
    mode: "onBlur",
    resolver: zodResolver(residencySchema),
    defaultValues: {
      streetNameAndNumber: "",
      city: "",
      postalCode: "",
      barangay: "",
      subdivisionPurok: "",
      province: "",
      country: "PH",
      ownerOrLessee: "",
      occupiedSince: "",
      sameAsCurrent: true,
      permStreetNameAndNumber: "",
      permCity: "",
      permPostalCode: "",
      permBarangay: "",
      permSubdivisionPurok: "",
      permProvince: "",
      permCountry: "PH",
      permOwnerOrLessee: "",
      permOccupiedSince: "",
      ...initial,
    },
  });

  function handleToggle(checked: boolean) {
    setSameAsCurrent(checked);
    setValue("sameAsCurrent", checked);
    if (checked) {
      const fields = [
        "streetNameAndNumber", "city", "postalCode", "barangay",
        "subdivisionPurok", "province", "country", "ownerOrLessee",
        "occupiedSince",
      ] as const;
      for (const f of fields) {
        const val = (document.getElementById(`current-${f}`) as HTMLInputElement)?.value;
        if (val) setValue(`perm${f.charAt(0).toUpperCase() + f.slice(1)}` as any, val);
      }
    }
  }

  function renderAddressInput(
    prefix: string,
    label: string,
    field: string,
    required?: boolean
  ) {
    const id = `${prefix}-${field}`;
    const registerName = (prefix === "perm" ? "perm" : "") +
      field.charAt(0).toUpperCase() + field.slice(1);
    return (
      <div>
        <label htmlFor={id} className="block text-sm font-medium">
          {label} {required && "*"}
        </label>
        <input
          id={id}
          {...register(registerName as any)}
          className="w-full border rounded p-2"
        />
        {(errors as any)[registerName] && (
          <p className="text-red-700 text-xs">{(errors as any)[registerName]?.message}</p>
        )}
      </div>
    );
  }

  return (
    <form onSubmit={handleSubmit(onNext)} className="space-y-6">
      <fieldset className="border rounded p-4">
        <legend className="font-semibold px-2">Current Address</legend>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {renderAddressInput("current", "Street", "streetNameAndNumber", true)}
          {renderAddressInput("current", "City", "city", true)}
          {renderAddressInput("current", "Postal Code", "postalCode", true)}
          {renderAddressInput("current", "Barangay", "barangay", true)}
          {renderAddressInput("current", "Subdivision / Purok", "subdivisionPurok")}
          {renderAddressInput("current", "Province", "province", true)}
          {renderAddressInput("current", "Country", "country", true)}
          {renderAddressInput("current", "Owner / Lessee", "ownerOrLessee", true)}
          {renderAddressInput("current", "Occupied Since", "occupiedSince", true)}
        </div>
      </fieldset>

      <label htmlFor="sameAsCurrent" className="flex items-center gap-2 cursor-pointer">
        <input
          id="sameAsCurrent"
          type="checkbox"
          checked={sameAsCurrent}
          onChange={(e) => handleToggle(e.target.checked)}
          className="w-5 h-5"
        />
        <span className="text-sm font-medium">Permanent address same as current</span>
      </label>

      {!sameAsCurrent && (
        <fieldset className="border rounded p-4">
          <legend className="font-semibold px-2">Permanent Address</legend>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {renderAddressInput("perm", "Street", "streetNameAndNumber", true)}
            {renderAddressInput("perm", "City", "city", true)}
            {renderAddressInput("perm", "Postal Code", "postalCode", true)}
            {renderAddressInput("perm", "Barangay", "barangay", true)}
            {renderAddressInput("perm", "Subdivision / Purok", "subdivisionPurok")}
            {renderAddressInput("perm", "Province", "province", true)}
            {renderAddressInput("perm", "Country", "country", true)}
            {renderAddressInput("perm", "Owner / Lessee", "ownerOrLessee", true)}
            {renderAddressInput("perm", "Occupied Since", "occupiedSince", true)}
          </div>
        </fieldset>
      )}

      <div className="flex justify-between">
        <button type="button" onClick={onBack} className="bg-gray-300 px-6 py-2 rounded hover:bg-gray-400">
          Back
        </button>
        <button type="submit" className="bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700">
          Next
        </button>
      </div>
    </form>
  );
}
