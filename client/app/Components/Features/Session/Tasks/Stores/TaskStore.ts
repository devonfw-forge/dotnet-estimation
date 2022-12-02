import axios from "axios";
import produce from "immer";
import create from "zustand";
import { IEstimation, ITask } from "../../../../../../app/Interfaces/ITask";
import { baseUrl, serviceUrl } from "../../../../../Constants/url";
import { IEstimationDto } from "../../../../../Interfaces/IEstimationDto";
import { Status } from "../../../../../Types/Status";

interface ISessionTaskState {
  tasks: ITask[];
  setCurrentTasks(payload: ITask[]): void;
  clearCurrentTasks: () => void;
  upsertTask: (task: ITask) => void;
  deleteTask: (id: String) => void;
  changeStatusOfTask: (id: String, status: Status) => void;
  userAlreadyVoted: (userId: String, taskId: String) => boolean;
  findOpenTask: () => ITask | undefined;
  upsertEstimationToTask: (estimation: IEstimationDto) => void;
}

export const useTaskStore = create<ISessionTaskState>()((set, get) => ({
  tasks: [],
  setCurrentTasks: (payload: ITask[]) =>
    set((state) => ({ ...state, tasks: payload })),
  clearCurrentTasks: () => set((state) => ({ ...state, tasks: [] })),
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
  deleteTask: (id: String) => {
    set(
      produce((draft: ISessionTaskState) => {
        // returns -1 if task isn't in list
        const index = draft.tasks.findIndex((task) => task.id == id);

        // prevents accidentally deleting the last task in the list
        if (index != -1) {
          draft.tasks.splice(index, 1);
        }
      })
    );
  },
  changeStatusOfTask: (id: String, status: Status) => {
    set(
      produce((draft: ISessionTaskState) => {
        draft.tasks.forEach((item) => {
          if (item.id == id) {
            item.status = status;
          }
        });
      })
    );
  },
  userAlreadyVoted: (userId: String, taskId: String) => {
    const task = get().tasks.find((item) => item.id == taskId);

    if (task) {
      const existingVote = task.estimations.find(
        (item) => item.voteBy == userId
      );

      if (existingVote) return true;
    }

    return false;
  },
  findOpenTask: () => {
    return get().tasks.find((item) => item.status == Status.Open);
  },
  upsertEstimationToTask: (estimation: IEstimationDto) => {
    set(
      produce((draft: ISessionTaskState) => {
        draft.tasks.forEach((item) => {
          if (item.id == estimation.taskId) {
            let gotReplaced = false;

            item.estimations.forEach((elem, index) => {
              if (elem.voteBy == estimation.voteBy) {
                item.estimations[index] = estimation;

                gotReplaced = true;
              }
            });

            if (!gotReplaced) {
              item.estimations.push({
                voteBy: estimation.voteBy,
                complexity: estimation.complexity,
              });
            }
          }
        });
      })
    );
  },
}));

const sortTasks = (tasks: ITask[]) => {
  const current = tasks.find(
    (item) => item.status == Status.Open || item.status == Status.Evaluated
  );

  const sortedTasks = [];

  if (current) {
    sortedTasks.push(current);
  }

  const supsendedTasks = tasks.filter(
    (item) => item.status == Status.Suspended
  );
  const endedTasks = tasks.filter((item) => item.status == Status.Ended);

  // TODO: sort using creation date

  sortedTasks.concat(supsendedTasks);
  sortedTasks.concat(endedTasks);

  return sortedTasks;
};
