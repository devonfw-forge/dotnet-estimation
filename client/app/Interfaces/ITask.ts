import { Status } from "../Types/Status";

export interface ITask {
  id: String;
  title: String;
  description?: String;
  url?: String;
  status: Status;
}

export interface ITaskStatusChange {
  id: String;
  status: Status;
}
