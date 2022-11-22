import { FunctionComponent } from "react";
import { ITask } from "../../../../../Interfaces/ITask";
import { Status } from "../../../../../Types/Status";
import { useTaskStore } from "../Stores/TaskStore";
import { TaskCard } from "./TaskCard";
import { TaskCreationForm } from "./TaskCreationForm";

export const TaskView: FunctionComponent<{}> = () => {
  const { tasks } = useTaskStore();

  return (
    <>
      <div
        className="h-full grid grid-rows-7 gap-4 justify-between bg-gray-100 flex-grow-0"
        style={{ minWidth: "25%" }}
      >
        <p className="p-3 link-dark text-decoration-none border-bottom text-center">
          Tasks
        </p>
        <div
          className="row-span-5 border-bottom overflow-y-scroll flex-grow-1 flex-shrink-0 min-h-0"
          style={{ flex: 1 }}
        >
          {tasks.map((item) => (
            <TaskCard
              key={"taskCard" + item.id}
              id={item.id}
              title={item.title}
              description={item.description}
              url={item.url}
              status={item.status}
            />
          ))}
        </div>
        <TaskCreationForm key={"taskCreateFormSingleton"} />
      </div>
    </>
  );
};
