import axios from "axios";
import produce from "immer";
import create from "zustand";
import { ITask } from "../../../../../../app/Interfaces/ITask";
import { baseUrl, serviceUrl } from "../../../../../Constants/url";
import { Status } from "../../../../../Types/Status";

interface ISessionTaskState {
  tasks: ITask[];
  setCurrentTasks(payload: ITask[]): void;
  clearCurrentTasks: () => void;
  upsertTask: (task: ITask) => void;
  changeStatusOfTask: (id: String, status: Status) => void;
  fetch: (id: String) => void;
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
  fetch: (id: String) => {
    async () => {
      const response = await axios({
        method: "get",
        url: baseUrl + serviceUrl + id + "/status",
        responseType: "stream",
      });

      if (response.status == 200) {
        set({ tasks: response.data.Tasks });
      }
    };
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
