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

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  email: string;
  role: string;
}

export interface MemberListItem {
  id: string;
  firstName: string;
  middleName?: string;
  lastName: string;
  emailAddress: string;
  employeeLevel: string;
  status: string;
  createdOn: string;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface MemberDetail {
  id: string;
  personalInfo: {
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
  };
  contactInfo: {
    emailAddress: string;
    contactNumber: string;
  };
  relatedPersons: {
    spouse?: { firstName: string; middleName?: string; lastName: string };
    motherMaidenName?: string;
    father?: { firstName: string; middleName?: string; lastName: string; suffix?: string };
  };
  governmentIds: {
    tin: string;
    sss: string;
  };
  primaryId: {
    type: string;
    number: string;
    issueDate: string;
    expiryDate: string;
    issueCountry: string;
  };
  currentAddress: AddressDto;
  permanentAddress: {
    sameAsCurrent: boolean;
    address?: AddressDto;
  };
  emergencyContact: {
    contactName: string;
    relationship: string;
    contactNumber: string;
  };
  employment: EmploymentDto;
  consent: {
    consentGiven: boolean;
    attestation: boolean;
    signatureName: string;
  };
  status: {
    code: string;
    displayName: string;
  };
}
