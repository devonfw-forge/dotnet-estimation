import { FunctionComponent } from "react";
import { useTaskStore } from "../Stores/TaskStore";
import { TaskCard } from "./TaskCard";
import { TaskCreationForm } from "./TaskCreationForm";

export const TaskView: FunctionComponent<{}> = () => {
  const { getCurrentTasks } = useTaskStore();

  return (
    <>
      <div
        className="h-full grid grid-rows-7 gap-4 justify-between bg-gray-100 flex-grow-0"
        style={{ width: "25%" }}
      >
        <p className="p-3 link-dark text-decoration-none border-bottom text-center">
          Tasks
        </p>
        <div
          className="row-span-5 border-bottom overflow-y-scroll flex-grow-1 flex-shrink-0 min-h-0"
          style={{ flex: 1 }}
        >
          {getCurrentTasks().map((item) => (
            <TaskCard
              key={"taskCard" + item.id}
              id={item.id}
              title={item.title}
              issue={item.issueNumber}
              description={item.description}
              isActive={item.isActive}
            />
          ))}
        </div>
        <TaskCreationForm key={"taskCreateFormSingleton"} />
      </div>
    </>
  );
};
