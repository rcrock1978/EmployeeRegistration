import { useState, useCallback } from "react";

export interface WizardState {
  currentStep: number;
  personalInfo: Record<string, unknown>;
  family: Record<string, unknown>;
  governmentIds: Record<string, unknown>;
  residency: Record<string, unknown>;
  employmentConsent: Record<string, unknown>;
}

const initialState: WizardState = {
  currentStep: 0,
  personalInfo: {},
  family: {},
  governmentIds: {},
  residency: {},
  employmentConsent: {},
};

export function useRegistration() {
  const [state, setState] = useState<WizardState>(initialState);
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submittedId, setSubmittedId] = useState<string | null>(null);

  const goToStep = useCallback((step: number) => {
    setState((prev) => ({ ...prev, currentStep: step }));
  }, []);

  const saveStepData = useCallback(
    (step: number, data: Record<string, unknown>) => {
      const keys = [
        "personalInfo",
        "family",
        "governmentIds",
        "residency",
        "employmentConsent",
      ] as const;
      const key = keys[step];
      setState((prev) => ({ ...prev, [key]: { ...prev[key], ...data } }));
    },
    []
  );

  const reset = useCallback(() => {
    setState(initialState);
    setSubmitting(false);
    setSubmitError(null);
    setSubmittedId(null);
  }, []);

  return {
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
  };
}
