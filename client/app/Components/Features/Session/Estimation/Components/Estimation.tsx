import axios from "axios";
import { useRouter } from "next/router";
import { FunctionComponent, useEffect, useState } from "react";
import { baseUrl, serviceUrl } from "../../../../../Constants/url";
import { ITask } from "../../../../../Interfaces/ITask";
import { EstimationType } from "../../../../../Types/EstimationType";
import { Status } from "../../../../../Types/Status";
import { Type } from "../../../../../Types/Type";
import { Center } from "../../../../Globals/Center";
import { useTaskStore } from "../../Tasks/Stores/TaskStore";
import { useEstimationStore, useFinalValueStore } from "../Stores/EstimationStore";
import {EstimationBar} from "./EstimationBar";
//import {ITaskresultDto} from "/Interfaces/ITaskResultDto";
import { requestStatusChange} from "../../Tasks/Components/TaskCard";


interface EstimationProps {
  id: String;
}

export const Estimation: FunctionComponent<EstimationProps> = ({ id }) => {
  const { findOpenTask, tasks, userAlreadyVoted, findEvaluatedTask, findClosedTask } = useTaskStore();
  const { complexity, effort, risk, resetStore } = useEstimationStore();
  const { finalValue } = useFinalValueStore();
  const columns = new Array<String>();

  const [doVote, setDoVote] = useState<boolean>(true);
  const [averageExists, setAverageExists] = useState<boolean>(false);

  const task = findOpenTask();

  const closedTask = findClosedTask();

  //check if an evaluated task existst
  const evaluatedTaskExists = findEvaluatedTask();

  let alreadyVoted = false;

  //averageComplexity value of the current evaluated task
  let averageComplexity = findEvaluatedTask()?.complexityAverage;



  if (task) {
    alreadyVoted = userAlreadyVoted("me", task.id);
  }

  useEffect(() => {
    if (alreadyVoted === true) {
      setDoVote(false);
      
    } else {
      setDoVote(true);
        
    }
    
        
    if(averageComplexity===undefined||averageComplexity===null) {
        setAverageExists(false);
    }
    else {
        setAverageExists(true);
    }


  }, [alreadyVoted, averageComplexity]);
  


  for (const type in EstimationType) {
    columns.push(type);
    console.log(type);
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
    
    if (result.status == 201 ) {
      // finally remove task from store
      // setDoVote to render the correct button after giving an estimation
      setDoVote(false);
      resetStore();
    }

  };
  
  const submitFinalResultToRestApi = async (taskId: String) => {
    let res = { amountOfVotes: 0, complexityAverage: averageComplexity , finalValue: 5 }; //averageComplexity, finalValue

    console.log(res);

    console.log("STATUSSS" + findEvaluatedTask()?.status);
    const rating = { id: findEvaluatedTask().id, status: Status.Ended ,  result: res  }; // fehler wg. namen finalComplVal?
    
    console.log("rating object :" + JSON.stringify(rating));

    const url = baseUrl + serviceUrl + id + "/task/status";

    const result = await axios({
      method: "put",
      url: url,
      data: rating,
    });

    if (result.status == 201 ) {
      resetStore();
      console.log("YOYOOYOYOYOOY");
    }
  }
  
  
  /*
  const requestStatusChange = async () => {
    const url = baseUrl + serviceUrl + id + "/task/status";

    
    // const result = newStatus === Status.Ended ? finalResult : null;
    const result = { amoutOfVotes: 0, complexityAverage: 0, finalValue: 5 } ;
    console.log("Final Result: " + result);

    await axios({
      method: "put",
      url: url,
      data: { id: findEvaluatedTask().id, status: Status.Ended , result: result },
    });
  };
  */

  if (tasks == undefined) {
    return <></>;
  } 

  const user = "me";    


  const renderComplexityAverageAndFinalValueChoice = () => (
      
      <div>
        <> 
        Average Complexity for the task  &nbsp;
        &apos;{findEvaluatedTask()?.title}&apos;: &nbsp;
        </>
          <strong>
            {averageComplexity}
          </strong>
            <>
              {renderFinalValueChoice(task)}
            </>
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

  const renderVoting = () => {
  
    if (task) { 
      if (doVote) { 
        return(
        renderEstimationForTask(task) 
        )
      }
      else { 
        return ( 
          renderVoteAgainButton()
        )
      }
    }
    else if ( evaluatedTaskExists ) {
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
               background:"#f1f4f6",
             }}
           >
             <p style={{ color: "#404b56" }}>
              Complexity :
             </p>
             <EstimationBar
               key={"estimationBar" + "Complexity"}
               // @ts-ignore
               type={EstimationType["Complexity"] as EstimationType}
             />
           </div>
         <div className="flex justify-center">
           <button
             onClick={() => submitFinalResultToRestApi(task)} //submitfinalresulto...
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

const renderWaitForLobbyhostMessage = ()  => (
    
  <strong className={defaultPadding}>
    
    Please wait for your lobby host to create a task!
  </strong>
    
);




  return renderVoting();
 
  

  /*
  return userHasAlreadyVoted
    ? renderViewWhenUserHasAlreadyVoted()
    : renderVoting();

    */
};
