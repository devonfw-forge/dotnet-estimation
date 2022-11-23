export enum Status {
  Open = 0,
  Evaluated = 1,
  Suspended = 2,
  Ended = 3,
}

export const convertStatusToNumber = (status: Status) => {
  switch (status) {
    case Status.Open:
      return 0;
    case Status.Evaluated:
      return 1;
    case Status.Suspended:
      return 2;
    case Status.Ended:
      return 3;
    default:
      return -1;
  }
};

export const convertStatusToColor = (status: Status) => {
  switch (status) {
    case Status.Open:
      return "bg-blue-400";
    case Status.Evaluated:
      return "bg-red-500";
    case Status.Suspended:
      return "bg-yellow-500";
    case Status.Ended:
      return "bg-blue-gray-400";
    default:
      return "bg-blue-gray-400";
  }
};

export const convertStatusToBorderColor = (status: Status) => {
  switch (status) {
    case Status.Open:
      return "border-l-blue-400";
    case Status.Evaluated:
      return "border-l-red-500";
    case Status.Suspended:
      return "border-l-yellow-500";
    case Status.Ended:
      return "border-l-blue-gray-400";
    default:
      return "border-l-blue-gray-400";
  }
};

export const convertStatusToTextColor = (status: Status) => {
  switch (status) {
    case Status.Open:
      return "text-blue-400";
    case Status.Evaluated:
      return "text-red-500";
    case Status.Suspended:
      return "text-yellow-500";
    case Status.Ended:
      return "text-slate-800/[.6]";
    default:
      return "text-slate-800/[.6]";
  }
};
