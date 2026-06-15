import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { familySchema, type FamilyData } from "../../lib/schemas";

interface Props {
  initial: Partial<FamilyData>;
  civilStatus: string;
  onNext: (data: FamilyData) => void;
  onBack: () => void;
}

export default function FamilyStep({ initial, civilStatus, onNext, onBack }: Props) {
  const isMarried = civilStatus === "Married";

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FamilyData>({
    mode: "onBlur",
    resolver: zodResolver(familySchema),
    defaultValues: {
      spouse: isMarried ? { firstName: "", middleName: "", lastName: "" } : undefined,
      motherMaidenName: "",
      father: undefined,
      ...initial,
    },
  });

  return (
    <form onSubmit={handleSubmit(onNext)} className="space-y-6">
      {isMarried && (
        <fieldset className="border rounded p-4">
          <legend className="font-semibold px-2">Spouse</legend>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label htmlFor="spouse.firstName" className="block text-sm font-medium">First Name *</label>
              <input id="spouse.firstName" {...register("spouse.firstName")} className="w-full border rounded p-2" />
              {errors.spouse?.firstName && <p className="text-red-700 text-xs">{errors.spouse.firstName.message}</p>}
            </div>
            <div>
              <label htmlFor="spouse.middleName" className="block text-sm font-medium">Middle Name</label>
              <input id="spouse.middleName" {...register("spouse.middleName")} className="w-full border rounded p-2" />
            </div>
            <div>
              <label htmlFor="spouse.lastName" className="block text-sm font-medium">Last Name *</label>
              <input id="spouse.lastName" {...register("spouse.lastName")} className="w-full border rounded p-2" />
              {errors.spouse?.lastName && <p className="text-red-700 text-xs">{errors.spouse.lastName.message}</p>}
            </div>
          </div>
        </fieldset>
      )}

      <fieldset className="border rounded p-4">
        <legend className="font-semibold px-2">Mother</legend>
        <div>
          <label htmlFor="motherMaidenName" className="block text-sm font-medium">Mother's Maiden Name</label>
          <input id="motherMaidenName" {...register("motherMaidenName")} className="w-full border rounded p-2" />
        </div>
      </fieldset>

      <fieldset className="border rounded p-4">
        <legend className="font-semibold px-2">Father</legend>
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label htmlFor="father.firstName" className="block text-sm font-medium">First Name</label>
            <input id="father.firstName" {...register("father.firstName")} className="w-full border rounded p-2" />
          </div>
          <div>
            <label htmlFor="father.middleName" className="block text-sm font-medium">Middle Name</label>
            <input id="father.middleName" {...register("father.middleName")} className="w-full border rounded p-2" />
          </div>
          <div>
            <label htmlFor="father.lastName" className="block text-sm font-medium">Last Name</label>
            <input id="father.lastName" {...register("father.lastName")} className="w-full border rounded p-2" />
          </div>
          <div>
            <label htmlFor="father.suffix" className="block text-sm font-medium">Suffix</label>
            <select id="father.suffix" {...register("father.suffix")} className="w-full border rounded p-2">
              <option value="">None</option>
              <option>Jr.</option>
              <option>Sr.</option>
              <option>II</option>
              <option>III</option>
            </select>
          </div>
        </div>
      </fieldset>

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
