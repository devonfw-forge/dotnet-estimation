import produce from "immer";
import { takeCoverage } from "v8";
import create from "zustand";
import { IResult } from "../../../../../Interfaces/IResult";
import { ITask } from "../../../../../Interfaces/ITask";

type SessionValue = ITask | IResult;

interface ISessionTaskState {
  tasks: ITask[];
  setCurrentTasks(payload: ITask[]): void;
  clearCurrentTasks: () => void;
  getCurrentTasks: () => ITask[];
  markAsActive: (id: String) => void;
  unmarkAll: () => void;
  upsertTask: (task: ITask) => void;
}

export const useTaskStore = create<ISessionTaskState>()((set, get) => ({
  tasks: [],
  setCurrentTasks: (payload: ITask[]) =>
    set((state) => ({ ...state, tasks: payload })),
  clearCurrentTasks: () => set((state) => ({ ...state, tasks: [] })),
  getCurrentTasks: () => {
    return get().tasks;
  },
  markAsActive: (id: String) => {
    set(
      produce((draft: ISessionTaskState) => {
        draft.tasks.forEach((task, index) => {
          if (task.id == id) {
            draft.tasks[index].isActive = true;
          } else {
            draft.tasks[index].isActive = false;
          }
        });
      })
    );
  },
  unmarkAll: () => {
    set(
      produce((draft: ISessionTaskState) => {
        draft.tasks.forEach((item) => {
          item.isActive = false;
        });
      })
    );
  },
  upsertTask: (task: ITask) => {
    set(
      produce((draft: ISessionTaskState) => {
        let taskGotUpserted = false;

        draft.tasks.forEach((item, index) => {
          if (task.id == item.id) {
            draft.tasks[index] = task;

            taskGotUpserted = true;
          }
        });

        if (taskGotUpserted == false) {
          draft.tasks.push(task);
        }
      })
    );
  },
}));
