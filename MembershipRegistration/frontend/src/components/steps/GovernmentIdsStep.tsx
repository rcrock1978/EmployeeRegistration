import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { governmentIdsSchema, type GovernmentIdsData } from "../../lib/schemas";

interface Props {
  initial: Partial<GovernmentIdsData>;
  onNext: (data: GovernmentIdsData) => void;
  onBack: () => void;
}

const PRIMARY_ID_TYPES = [
  "Passport", "Driver's License", "UMID", "SSS ID", "GSIS ID",
  "PRC ID", "Voter's ID", "PhilHealth ID", "National ID", "Postal ID", "Others",
];

export default function GovernmentIdsStep({ initial, onNext, onBack }: Props) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<GovernmentIdsData>({
    mode: "onBlur",
    resolver: zodResolver(governmentIdsSchema),
    defaultValues: {
      tin: "",
      sss: "",
      primaryId: { type: "", number: "", issueDate: "", expiryDate: "", issueCountry: "PH" },
      ...initial,
    },
  });

  return (
    <form onSubmit={handleSubmit(onNext)} className="space-y-6">
      <fieldset className="border rounded p-4">
        <legend className="font-semibold px-2">Government IDs</legend>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium">TIN *</label>
            <input
              {...register("tin")}
              placeholder="123-456-789-000"
              className="w-full border rounded p-2"
              inputMode="numeric"
            />
            {errors.tin && <p className="text-red-500 text-xs">{errors.tin.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium">SSS *</label>
            <input
              {...register("sss")}
              placeholder="01-2345678-9"
              className="w-full border rounded p-2"
              inputMode="numeric"
            />
            {errors.sss && <p className="text-red-500 text-xs">{errors.sss.message}</p>}
          </div>
        </div>
      </fieldset>

      <fieldset className="border rounded p-4">
        <legend className="font-semibold px-2">Primary ID</legend>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium">ID Type *</label>
            <select {...register("primaryId.type")} className="w-full border rounded p-2">
              <option value="">Select</option>
              {PRIMARY_ID_TYPES.map((t) => <option key={t}>{t}</option>)}
            </select>
            {errors.primaryId?.type && <p className="text-red-500 text-xs">{errors.primaryId.type.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium">ID Number *</label>
            <input {...register("primaryId.number")} className="w-full border rounded p-2" />
            {errors.primaryId?.number && <p className="text-red-500 text-xs">{errors.primaryId.number.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium">Issue Date *</label>
            <input type="date" {...register("primaryId.issueDate")} className="w-full border rounded p-2" />
            {errors.primaryId?.issueDate && <p className="text-red-500 text-xs">{errors.primaryId.issueDate.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium">Expiry Date *</label>
            <input type="date" {...register("primaryId.expiryDate")} className="w-full border rounded p-2" />
            {errors.primaryId?.expiryDate && <p className="text-red-500 text-xs">{errors.primaryId.expiryDate.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium">Issue Country *</label>
            <input {...register("primaryId.issueCountry")} className="w-full border rounded p-2" />
            {errors.primaryId?.issueCountry && <p className="text-red-500 text-xs">{errors.primaryId.issueCountry.message}</p>}
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
