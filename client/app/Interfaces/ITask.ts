import { Status } from "../Types/Status";

export interface IEstimation {
  voteBy: String;
  complexity: Number;
}

export interface ITask {
  id: String;
  title: String;
  description?: String;
  url?: String;
  status: Status;
  estimations: IEstimation[];
}

export interface ITaskStatusChange {
  id: String;
  status: Status;
}
