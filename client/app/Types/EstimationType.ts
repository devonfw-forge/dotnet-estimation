export enum EstimationType {
  Complexity = "complexity",
  Effort = "effort",
  Risk = "risk",
}

export const convertTypeToColorIfActive = (
  type: EstimationType,
  isActive: boolean
) => {
  if (!isActive) {
    return "#f1f4f6";
  }

  // @ts-ignore
  switch (type) {
    case EstimationType.Complexity: {
      return "#16aaff";
    }
    case EstimationType.Effort: {
      return "#0d6efd";
    }
    case EstimationType.Risk: {
      return "#d92550";
    }
  }
};

export const convertIndexToType = (index: number) => {
  switch (index) {
    case 0: {
      return EstimationType.Complexity;
    }
    case 1: {
      return EstimationType.Effort;
    }
    case 2: {
      return EstimationType.Risk;
    }
  }
};
