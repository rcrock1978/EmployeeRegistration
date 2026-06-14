export interface Envelope<T> {
  isSuccess: boolean;
  value: T | null;
  error: AppError | null;
}

export interface AppError {
  code: string;
  message: string;
  details?: FieldError[];
}

export interface FieldError {
  field: string;
  code: string;
  message: string;
}

export interface RegisterMemberResponse {
  id: string;
}

export interface RegisterRequest {
  personalInfo: PersonalInfoDto;
  contactInfo: ContactInfoDto;
  relatedPersons: RelatedPersonsDto;
  governmentIds: GovernmentIdsDto;
  primaryId: PrimaryIdDto;
  currentAddress: AddressDto;
  permanentAddress: PermanentAddressDto;
  emergencyContact: EmergencyContactDto;
  employment: EmploymentDto;
  consent: ConsentDto;
}

export interface PersonalInfoDto {
  title: string;
  firstName: string;
  middleName?: string;
  lastName: string;
  suffix?: string;
  alias?: string;
  dateOfBirth: string;
  placeOfBirth: string;
  countryOfBirth: string;
  nationality: string;
  gender: string;
  civilStatus: string;
  religion?: string;
  highestEducationalAttainment: string;
  numberOfDependents: number;
}

export interface ContactInfoDto {
  emailAddress: string;
  contactNumber: string;
}

export interface RelatedPersonsDto {
  spouse?: SpouseDto;
  motherMaidenName?: string;
  father?: FatherDto;
}

export interface SpouseDto {
  firstName: string;
  middleName?: string;
  lastName: string;
}

export interface FatherDto {
  firstName: string;
  middleName?: string;
  lastName: string;
  suffix?: string;
}

export interface GovernmentIdsDto {
  tin: string;
  sss: string;
}

export interface PrimaryIdDto {
  type: string;
  number: string;
  issueDate: string;
  expiryDate: string;
  issueCountry: string;
}

export interface AddressDto {
  streetNameAndNumber: string;
  city: string;
  postalCode: string;
  barangay: string;
  subdivisionPurok?: string;
  province: string;
  country: string;
  ownerOrLessee: string;
  occupiedSince: string;
}

export interface PermanentAddressDto {
  sameAsCurrent: boolean;
  address?: AddressDto;
}

export interface EmergencyContactDto {
  contactName: string;
  relationship: string;
  contactNumber: string;
}

export interface EmploymentDto {
  employeeLevel: string;
  companyTradeName: string;
  companyIdNumber: string;
  grossIncome: number;
  incomePeriod: string;
  occupation: string;
  hiredFrom: string;
  hiredTo?: string;
}

export interface ConsentDto {
  consentGiven: boolean;
  attestation: boolean;
  signatureName: string;
}
