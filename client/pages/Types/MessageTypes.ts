import { IResult } from "../Interfaces/IResult";
import { ITask } from "../Interfaces/ITask";
import { Action } from "./Action";

export type StartedTaskEstimation = {
  action: Action.StartedTaskEstimation;
  title?: String;
  description?: String;
} & ITask;

export type StoppedTaskEstimation = {
  action: Action.StoppedTaskEstimation;
} & ITask;

export type StoppedSession = {
  action: Action.StoppedSession;
  results: IResult;
};

export type IncomingMessage =
  | StartedTaskEstimation
  | StoppedTaskEstimation
  | StoppedSession;
