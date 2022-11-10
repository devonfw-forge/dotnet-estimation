export enum Type {
  Complexity = "complexity",
  Effort = "effort",
  Risk = "risk",
}

export const convertTypeToColorIfActive = (type: Type, isActive: boolean) => {
  if (!isActive) {
    return "#f1f4f6";
  }

  // @ts-ignore
  switch (type) {
    case Type.Complexity: {
      return "#16aaff";
    }
    case Type.Effort: {
      return "#0d6efd";
    }
    case Type.Risk: {
      return "#d92550";
    }
  }
};

export const convertIndexToType = (index: number) => {
  switch (index) {
    case 0: {
      return Type.Complexity;
    }
    case 1: {
      return Type.Effort;
    }
    case 2: {
      return Type.Risk;
    }
  }
};
