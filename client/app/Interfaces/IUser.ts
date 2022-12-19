import { Role } from "../Types/Role";

export interface IUser {
  id: String;
  username: String;
  imageUrl?: String;
  online?: boolean;
  role: Role;
  token: string;
}
