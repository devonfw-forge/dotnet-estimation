import { FunctionComponent } from "react";
import { EstimationType } from "../../../../../Types/EstimationType";
import { useTaskStore } from "../../Tasks/Stores/TaskStore";
import { useEstimationStore } from "../Stores/EstimationStore";
import { EstimationBar } from "./EstimationBar";

export const Estimation: FunctionComponent<{}> = () => {
  const { complexity, effort, risk, resetStore } = useEstimationStore();
  const columns = new Array<String>();

  for (const type in EstimationType) {
    columns.push(type);
  }

  const defaultPadding = "p-4";

  const submitEstimationToRestApi = () => {
    console.log(complexity);
    console.log(effort);
    console.log(risk);

    // finally reset the store
    resetStore();

    // unmark current task as non-active
  };

  return (
    <div className="flex flex-col bg-white rounded">
      <div className={"flex flex-col items-center"}>
        <strong className={defaultPadding}>
          How would you rate this task?
        </strong>
      </div>
      <>
        {columns.map((type, index) => (
          <div
            key={"estimationColumn" + type}
            className={
              "flex flex-row justify-between items-center " + defaultPadding
            }
            style={{
              background: index % 2 == 0 ? "#f1f4f6" : "#fff",
            }}
          >
            <p style={{ color: "#404b56" }}>
              {type[0].toUpperCase() + type.slice(1) + ":"}
            </p>
            <EstimationBar
              key={"estimationBar" + type}
              // @ts-ignore
              type={EstimationType[type] as EstimationType}
            />
          </div>
        ))}
        <div className="flex justify-center">
          <button
            onClick={submitEstimationToRestApi}
            className={
              "border-b-blue-700 bg-blue-500 hover:bg-blue-700 text-white font-bold m-2 p-2 rounded "
            }
          >
            Submit
          </button>
        </div>
      </>
    </div>
  );
};
