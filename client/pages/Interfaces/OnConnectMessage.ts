import { ITask } from "./ITask";
import { IUser } from "./IUser";

export type OnConnectMessage = {
  tasks: ITask[];
  users: IUser[];
};
