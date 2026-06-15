import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { personalInfoSchema, type PersonalInfoData } from "../../lib/schemas";

interface Props {
  initial: Partial<PersonalInfoData>;
  onNext: (data: PersonalInfoData) => void;
}

const CIVIL_STATUS_OPTIONS = [
  "Single", "Married", "Divorced", "Separated", "Widowed",
];

const GENDER_OPTIONS = ["Male", "Female"];

export default function PersonalInfoStep({ initial, onNext }: Props) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<PersonalInfoData>({
    mode: "onBlur",
    resolver: zodResolver(personalInfoSchema),
    defaultValues: {
      title: "",
      firstName: "",
      middleName: "",
      lastName: "",
      suffix: "",
      alias: "",
      dateOfBirth: "",
      placeOfBirth: "",
      countryOfBirth: "PH",
      nationality: "Filipino",
      gender: "Male",
      civilStatus: "Single",
      religion: "",
      highestEducationalAttainment: "",
      numberOfDependents: 0,
      email: "",
      contactNumber: "",
      ...initial,
    },
  });

  return (
    <form onSubmit={handleSubmit(onNext)} className="space-y-6">
      <fieldset className="border rounded p-4">
        <legend className="font-semibold px-2">Name</legend>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label htmlFor="title" className="block text-sm font-medium">Title</label>
            <select id="title" {...register("title")} className="w-full border rounded p-2">
              <option value="">Select</option>
              <option>Mr.</option>
              <option>Ms.</option>
              <option>Mrs.</option>
              <option>Dr.</option>
              <option>Atty.</option>
            </select>
            {errors.title && <p className="text-red-700 text-xs">{errors.title.message}</p>}
          </div>
          <div>
            <label htmlFor="firstName" className="block text-sm font-medium">First Name *</label>
            <input id="firstName" {...register("firstName")} className="w-full border rounded p-2" />
            {errors.firstName && <p className="text-red-700 text-xs">{errors.firstName.message}</p>}
          </div>
          <div>
            <label htmlFor="middleName" className="block text-sm font-medium">Middle Name</label>
            <input id="middleName" {...register("middleName")} className="w-full border rounded p-2" />
          </div>
          <div>
            <label htmlFor="lastName" className="block text-sm font-medium">Last Name *</label>
            <input id="lastName" {...register("lastName")} className="w-full border rounded p-2" />
            {errors.lastName && <p className="text-red-700 text-xs">{errors.lastName.message}</p>}
          </div>
          <div>
            <label htmlFor="suffix" className="block text-sm font-medium">Suffix</label>
            <select id="suffix" {...register("suffix")} className="w-full border rounded p-2">
              <option value="">None</option>
              <option>Jr.</option>
              <option>Sr.</option>
              <option>II</option>
              <option>III</option>
              <option>IV</option>
            </select>
          </div>
          <div>
            <label htmlFor="alias" className="block text-sm font-medium">Alias</label>
            <input id="alias" {...register("alias")} className="w-full border rounded p-2" />
          </div>
        </div>
      </fieldset>

      <fieldset className="border rounded p-4">
        <legend className="font-semibold px-2">Demographics</legend>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label htmlFor="dateOfBirth" className="block text-sm font-medium">Date of Birth *</label>
            <input id="dateOfBirth" type="date" {...register("dateOfBirth")} className="w-full border rounded p-2" />
            {errors.dateOfBirth && <p className="text-red-700 text-xs">{errors.dateOfBirth.message}</p>}
          </div>
          <div>
            <label htmlFor="placeOfBirth" className="block text-sm font-medium">Place of Birth *</label>
            <input id="placeOfBirth" {...register("placeOfBirth")} className="w-full border rounded p-2" />
            {errors.placeOfBirth && <p className="text-red-700 text-xs">{errors.placeOfBirth.message}</p>}
          </div>
          <div>
            <label htmlFor="countryOfBirth" className="block text-sm font-medium">Country of Birth *</label>
            <input id="countryOfBirth" {...register("countryOfBirth")} className="w-full border rounded p-2" />
            {errors.countryOfBirth && <p className="text-red-700 text-xs">{errors.countryOfBirth.message}</p>}
          </div>
          <div>
            <label htmlFor="nationality" className="block text-sm font-medium">Nationality *</label>
            <input id="nationality" {...register("nationality")} className="w-full border rounded p-2" />
            {errors.nationality && <p className="text-red-700 text-xs">{errors.nationality.message}</p>}
          </div>
          <div>
            <label htmlFor="gender" className="block text-sm font-medium">Gender *</label>
            <select id="gender" {...register("gender")} className="w-full border rounded p-2">
              {GENDER_OPTIONS.map((o) => <option key={o}>{o}</option>)}
            </select>
          </div>
          <div>
            <label htmlFor="civilStatus" className="block text-sm font-medium">Civil Status *</label>
            <select id="civilStatus" {...register("civilStatus")} className="w-full border rounded p-2">
              {CIVIL_STATUS_OPTIONS.map((o) => <option key={o}>{o}</option>)}
            </select>
          </div>
          <div>
            <label htmlFor="religion" className="block text-sm font-medium">Religion</label>
            <input id="religion" {...register("religion")} className="w-full border rounded p-2" />
          </div>
          <div>
            <label htmlFor="highestEducationalAttainment" className="block text-sm font-medium">Highest Education *</label>
            <select id="highestEducationalAttainment" {...register("highestEducationalAttainment")} className="w-full border rounded p-2">
              <option value="">Select</option>
              <option>Elementary</option>
              <option>High School</option>
              <option>Vocational</option>
              <option>Bachelor's</option>
              <option>Master's</option>
              <option>Doctorate</option>
            </select>
            {errors.highestEducationalAttainment && (
              <p className="text-red-700 text-xs">{errors.highestEducationalAttainment.message}</p>
            )}
          </div>
          <div>
            <label htmlFor="numberOfDependents" className="block text-sm font-medium">Number of Dependents</label>
            <input id="numberOfDependents" type="number" {...register("numberOfDependents", { valueAsNumber: true })} className="w-full border rounded p-2" />
          </div>
        </div>
      </fieldset>

      <fieldset className="border rounded p-4">
        <legend className="font-semibold px-2">Contact Information</legend>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label htmlFor="email" className="block text-sm font-medium">Email Address *</label>
            <input id="email" type="email" {...register("email")} className="w-full border rounded p-2" inputMode="email" />
            {errors.email && <p className="text-red-700 text-xs">{errors.email.message}</p>}
          </div>
          <div>
            <label htmlFor="contactNumber" className="block text-sm font-medium">Contact Number *</label>
            <input id="contactNumber" type="tel" {...register("contactNumber")} className="w-full border rounded p-2" inputMode="tel" />
            {errors.contactNumber && <p className="text-red-700 text-xs">{errors.contactNumber.message}</p>}
          </div>
        </div>
      </fieldset>

      <div className="flex justify-end">
        <button type="submit" className="bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700">
          Next
        </button>
      </div>
    </form>
  );
}
