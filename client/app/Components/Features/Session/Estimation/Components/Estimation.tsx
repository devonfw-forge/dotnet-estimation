import axios from "axios";
import { FunctionComponent, useEffect, useState } from "react";
import { baseUrl, serviceUrl } from "../../../../../Constants/url";
import { ITask } from "../../../../../Interfaces/ITask";
import { EstimationType } from "../../../../../Types/EstimationType";
import { Role } from "../../../../../Types/Role";
import { Status } from "../../../../../Types/Status";
import { Center } from "../../../../Globals/Center";
import { useAuthStore } from "../../../Authentication/Stores/AuthStore";
import { useTaskStore } from "../../Tasks/Stores/TaskStore";
import { useEstimationStore } from "../Stores/EstimationStore";
import { EstimationBar } from "./EstimationBar";

interface EstimationProps {
  id: String;
}

export const Estimation: FunctionComponent<EstimationProps> = ({ id }) => {
  const { findOpenTask, tasks, userAlreadyVoted, findEvaluatedTask } = useTaskStore();
  const { complexity, effort, risk, resetStore } = useEstimationStore();
  const { userId, token, role } = useAuthStore();

  const columns = new Array<String>();

  const [doVote, setDoVote] = useState<boolean>(true);

  const task = findOpenTask();
  const evaluatedTaskExists = findEvaluatedTask();
  let averageComplexity = findEvaluatedTask()?.result?.complexityAverage;

  let alreadyVoted = false;

  if (task) {
    alreadyVoted = userAlreadyVoted(userId as string, task.id);
  }

  useEffect(() => {
    if (alreadyVoted === true) {
      setDoVote(false);
    } else {
      setDoVote(true);
    }
  }, [alreadyVoted]);


  for (const type in EstimationType) {
    columns.push(type);
  }

  const defaultPadding = "p-4";

  // TODO: replace voteby with user
  const submitEstimationToRestApi = async (taskId: String) => {
    console.log(userId);

    const rating = { taskId: taskId, voteBy: userId, complexity };

    const url = baseUrl + serviceUrl + id + "/estimation";

    const result = await axios({
      method: "post",
      url: url,
      data: rating,
      headers: {
        Accept: "application/json",
        "Content-Type": " application/json",
        Authorization: `Bearer ${token}`,
      },
    });

    if (result.status == 201) {
      // setDoVote to render the vote again button
      setDoVote(false);
      // finally reset the estimation buttons to 1
      resetStore();
    }
  };

  const submitFinalResultToRestApi = async () => {
    const evaluatedTask = findEvaluatedTask();

    if (evaluatedTask) {
      let res = { amountOfVotes: 0, complexityAverage: averageComplexity, finalValue: evaluatedTask.result.finalValue };

      const url = baseUrl + serviceUrl + id + "/task/status";

      const result = await axios({
        method: "put",
        url: url,
        data: { id: evaluatedTask.id, status: Status.Ended, result: res },
        headers: {
          Accept: "application/json",
          "Content-Type": " application/json",
          Authorization: `Bearer ${token}`,
        }
      });

      if (result.status == 200) {
        // reset the estimation buttons to 1
        resetStore();
      }
    }
  }

  if (!userId) {
    return <>You are currently not logged in!</>;
  }

  if (tasks == undefined) {
    return <></>;
  }

  const user = "me";

  const renderVoting = () => {
    if (task) {
      if (doVote) {
        return (
          renderEstimationForTask(task)
        )
      }
      else {
        return (
          renderVoteAgainButton()
        )
      }
    }
    else if (evaluatedTaskExists) {
      return (
        renderComplexityAverageAndFinalValueChoice()
      )
    }
    else {
      return (
        renderWaitForLobbyhostMessage()
      )
    }
  };

  const renderComplexityAverageAndFinalValueChoice = () => (
    <div>
      <>
        Average Complexity for the task &nbsp;
        &apos;{findEvaluatedTask()?.title}&apos;: &nbsp;
      </>
      <strong>
        {averageComplexity}
      </strong>
      {role === Role.Admin ? renderFinalValueChoice(task) : <></>}
    </div>
  );

  const renderEstimationForTask = (task: ITask) => (
    <Center>
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
                isFinal={false}
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
    </Center>
  );

  const renderFinalValueChoice = (task: ITask) => (
    <Center>
      <>
        <strong className={defaultPadding}>
          Choose final value for the Complexity
        </strong>
        <>
          <div
            key={"estimationColumn" + "Complexity"}
            className={
              "flex flex-row justify-between items-center " + defaultPadding
            }
            style={{
              background: "#f1f4f6",
            }}
          >
            <p style={{ color: "#404b56" }}>
              Complexity:
            </p>
            <EstimationBar
              key={"estimationBar" + "Complexity"}
              // @ts-ignore
              type={EstimationType["Complexity"] as EstimationType}
              isFinal={true}
            />
          </div>
          <div className="flex justify-center">
            <button
              onClick={() => submitFinalResultToRestApi()}
              className={
                "border-b-blue-700 bg-blue-500 hover:bg-blue-700 text-white font-bold m-2 p-2 rounded "
              }
            >
              Submit
            </button>
          </div>
        </>
      </>
    </Center>
  );

  const renderVoteAgainButton = () => (
    <div className="flex justify-center">
      <button
        onClick={() => {
          setDoVote(true);
        }}
        className={
          "border-b-blue-700 bg-blue-500 hover:bg-blue-700 text-white font-bold m-2 p-2 rounded "
        }
      >
        I want to vote again
      </button>
    </div>
  );

  const renderWaitForLobbyhostMessage = () => (
    <strong className={defaultPadding}>
      Please wait for your lobby host to create a task!
    </strong>
  );

  return renderVoting();
};
