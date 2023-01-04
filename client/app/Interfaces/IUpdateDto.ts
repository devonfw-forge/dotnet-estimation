import { IUser } from "./IUser";

export interface IUpdateDto extends Array<String> {
    user: IUser,
    availableClients: Array<String>
  }
  