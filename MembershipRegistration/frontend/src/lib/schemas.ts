import { z } from "zod/v4";

const tinPattern = /^\d{3}-\d{3}-\d{3}-\d{3}$/;
const sssPattern = /^\d{2}-\d{7}-\d{1}$/;

export const personalInfoSchema = z.object({
  title: z.string().min(1, "Title is required"),
  firstName: z.string().min(2).max(100),
  middleName: z.string().max(100).optional(),
  lastName: z.string().min(2).max(100),
  suffix: z.string().optional(),
  alias: z.string().optional(),
  dateOfBirth: z.string().min(1, "Date of birth is required"),
  placeOfBirth: z.string().min(1).max(200),
  countryOfBirth: z.string().min(1),
  nationality: z.string().min(1),
  gender: z.string().min(1),
  civilStatus: z.string().min(1),
  religion: z.string().optional(),
  highestEducationalAttainment: z.string().min(1),
  numberOfDependents: z.number().int().min(0),
});

export const familySchema = z.object({
  spouse: z
    .object({
      firstName: z.string().min(1),
      middleName: z.string().optional(),
      lastName: z.string().min(1),
    })
    .optional(),
  motherMaidenName: z.string().optional(),
  father: z
    .object({
      firstName: z.string().min(1),
      middleName: z.string().optional(),
      lastName: z.string().min(1),
      suffix: z.string().optional(),
    })
    .optional(),
});

export const governmentIdsSchema = z.object({
  tin: z.string().regex(tinPattern, "TIN must follow ###-###-###-### format"),
  sss: z.string().regex(sssPattern, "SSS must follow ##-#######-# format"),
  primaryId: z.object({
    type: z.string().min(1),
    number: z.string().min(1),
    issueDate: z.string().min(1),
    expiryDate: z.string().min(1),
    issueCountry: z.string().min(1),
  }),
});

export const residencySchema = z
  .object({
    streetNameAndNumber: z.string().min(1).max(200),
    city: z.string().min(1),
    postalCode: z.string().min(1),
    barangay: z.string().min(1),
    subdivisionPurok: z.string().optional(),
    province: z.string().min(1),
    country: z.string().min(1),
    ownerOrLessee: z.string().min(1),
    occupiedSince: z.string().min(1),
    sameAsCurrent: z.boolean(),
    permStreetNameAndNumber: z.string().optional(),
    permCity: z.string().optional(),
    permPostalCode: z.string().optional(),
    permBarangay: z.string().optional(),
    permSubdivisionPurok: z.string().optional(),
    permProvince: z.string().optional(),
    permCountry: z.string().optional(),
    permOwnerOrLessee: z.string().optional(),
    permOccupiedSince: z.string().optional(),
  })
  .refine(
    (data) => {
      if (data.sameAsCurrent) return true;
      return (
        !!data.permStreetNameAndNumber &&
        !!data.permCity &&
        !!data.permPostalCode &&
        !!data.permBarangay &&
        !!data.permProvince &&
        !!data.permCountry
      );
    },
    { message: "Permanent address fields are required when not same as current" }
  );

export const employmentConsentSchema = z.object({
  employeeLevel: z.string().min(1),
  companyTradeName: z.string().min(1),
  companyIdNumber: z.string().min(1),
  grossIncome: z.number().min(0),
  incomePeriod: z.string().min(1),
  occupation: z.string().min(1),
  hiredFrom: z.string().min(1),
  hiredTo: z.string().optional(),
  contactName: z.string().min(1),
  relationship: z.string().min(1),
  contactNumber: z.string().min(1),
  consentGiven: z.boolean().refine((v) => v === true, { message: "You must give consent to proceed" }),
  attestation: z.boolean().refine((v) => v === true, { message: "You must attest to proceed" }),
  signatureName: z.string().min(1),
});

export type PersonalInfoData = z.infer<typeof personalInfoSchema>;
export type FamilyData = z.infer<typeof familySchema>;
export type GovernmentIdsData = z.infer<typeof governmentIdsSchema>;
export type ResidencyData = z.infer<typeof residencySchema>;
export type EmploymentConsentData = z.infer<typeof employmentConsentSchema>;
