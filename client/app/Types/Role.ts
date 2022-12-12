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
