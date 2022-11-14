import { FunctionComponent } from "react";
import { useTaskStore } from "../Stores/TaskStore";

export const TaskCard: FunctionComponent<{
  id: String;
  title: String;
  url?: String;
  description?: String;
  isActive: boolean;
}> = ({ id, title, url, description, isActive }) => {
  const buildBorderColor = (isActive: boolean) => {
    return isActive ? " border-l-blue-400" : " border-l-blue-gray-400";
  };

  return (
    <>
      <div
        className={
          "flex flex-col border-l-4 m-2 p-4 bg-white rounded" +
          buildBorderColor(isActive)
        }
        onClick={() => {
          console.log("Marking task with id: " + id + " as active");
        }}
      >
        <div className="flex flex-row justify-between py-2">
          <strong>{title}</strong>
          <p>{url}</p>
        </div>
        <hr />
        <p className="pt-2">{description}</p>
      </div>
    </>
  );
};
