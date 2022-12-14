export enum Role {
  Admin = "Admin",
  Voter = "Voter",
  Spectator = "Spectator",
}

export const toRole = (role: String) => {
  switch (role) {
    case "Admin":
      return Role.Admin;
    case "Voter":
      return Role.Voter;
    case "Spectator":
      return Role.Spectator;
    default:
      return Role.Spectator;
  }
};

export const toNumber = (role: Role) => {
  switch (role) {
    case Role.Admin:
      return 0;
    case Role.Voter:
      return 1;
    case Role.Spectator:
      return 2;
    default:
      return -1;
  }
};
