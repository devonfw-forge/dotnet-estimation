import axios from "axios";
import { useRouter } from "next/router";
import { FunctionComponent, useEffect, useState } from "react";
import { baseUrl, serviceUrl } from "../../../../../Constants/url";
import { ITask } from "../../../../../Interfaces/ITask";
import { EstimationType } from "../../../../../Types/EstimationType";
import { Status } from "../../../../../Types/Status";
import { Center } from "../../../../Globals/Center";
import { useTaskStore } from "../../Tasks/Stores/TaskStore";
import { useEstimationStore } from "../Stores/EstimationStore";
import { EstimationBar } from "./EstimationBar";

interface EstimationProps {
  id: String;
}

export const Estimation: FunctionComponent<EstimationProps> = ({ id }) => {
  const { findOpenTask, tasks, userAlreadyVoted } = useTaskStore();
  const { complexity, effort, risk, resetStore } = useEstimationStore();
  const columns = new Array<String>();

  const [doVote, setDoVote] = useState<boolean>(true);

  const task = findOpenTask();

  let alreadyVoted = false;

  if (task) {
    alreadyVoted = userAlreadyVoted("me", task.id);
  }

  useEffect(() => {
    if (alreadyVoted === true) {
      setDoVote(false);
    } else {
      setDoVote(true);
    }
  }, [task, alreadyVoted]);

  for (const type in EstimationType) {
    columns.push(type);
  }

  const defaultPadding = "p-4";

  // TODO: replace voteby with user
  const submitEstimationToRestApi = async (taskId: String) => {
    const rating = { taskId: taskId, voteBy: "me", complexity };

    const url = baseUrl + serviceUrl + id + "/estimation";

    const result = await axios({
      method: "post",
      url: url,
      data: rating,
    });

    if (result.status == 200) {
      // finally remove task from store
      resetStore();
    }
  };

  if (tasks == undefined) {
    return <></>;
  }

  const user = "me";

  const renderEstimationForTask = (task: ITask) => {
    return (
      <>
        <strong className={defaultPadding}>
          How would you rate this task?
        </strong>
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
              onClick={() => submitEstimationToRestApi(task.id)}
              className={
                "border-b-blue-700 bg-blue-500 hover:bg-blue-700 text-white font-bold m-2 p-2 rounded "
              }
            >
              Submit
            </button>
          </div>
        </>
      </>
    );
  };

  const renderVoting = () => {
    return (
      <Center>
        {task ? (
          doVote ? (
            renderEstimationForTask(task)
          ) : (
            <div className="flex justify-center">
              <button
                onClick={() => {
                  setDoVote(true);
                }}
                className={
                  "border-b-blue-700 bg-blue-500 hover:bg-blue-700 text-white font-bold m-2 p-2 rounded "
                }
              >
                I want to vote again!
              </button>
            </div>
          )
        ) : (
          <strong className={defaultPadding}>
            Please wait for your lobby host to create a task!
          </strong>
        )}
      </Center>
    );
  };

  return renderVoting();

  /*
  return userHasAlreadyVoted
    ? renderViewWhenUserHasAlreadyVoted()
    : renderVoting();

    */
};
