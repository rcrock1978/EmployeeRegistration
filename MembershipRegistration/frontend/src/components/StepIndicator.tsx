const STEPS = [
  "Personal Info",
  "Family",
  "Government IDs",
  "Residency",
  "Review & Submit",
];

interface Props {
  currentStep: number;
  completedSteps: Set<number>;
}

export default function StepIndicator({ currentStep, completedSteps }: Props) {
  return (
    <nav aria-label="Registration progress" className="mb-8">
      <ol className="flex items-center justify-center gap-1 text-sm">
        {STEPS.map((label, idx) => {
          const isCompleted = completedSteps.has(idx);
          const isCurrent = idx === currentStep;
          return (
            <li key={idx} className="flex items-center gap-1">
              <span
                className={`inline-flex items-center justify-center w-8 h-8 rounded-full text-xs font-bold 
                  ${isCompleted ? "bg-green-500 text-white" : ""}
                  ${isCurrent ? "bg-blue-600 text-white ring-2 ring-blue-300" : ""}
                  ${!isCompleted && !isCurrent ? "bg-gray-200 text-gray-500" : ""}`}
              >
                {isCompleted ? "✓" : idx + 1}
              </span>
              <span
                className={`hidden sm:inline ${isCurrent ? "font-semibold text-blue-600" : "text-gray-500"}`}
              >
                {label}
              </span>
              {idx < STEPS.length - 1 && <span className="w-6 h-px bg-gray-300 mx-1" />}
            </li>
          );
        })}
      </ol>
    </nav>
  );
}
