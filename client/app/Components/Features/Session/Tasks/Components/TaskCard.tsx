import axios from "axios";
import { FunctionComponent, useState } from "react";
import {
  Status,
  convertStatusToNumber,
  convertStatusToColor,
  convertStatusToBorderColor,
  convertStatusToTextColor,
} from "../../../../../Types/Status";
import { useTaskStore } from "../Stores/TaskStore";
import { baseUrl, serviceUrl } from "../../../../../Constants/url";
import { Checkbox } from "./Checkmark";
import { useAuthStore } from "../../../Authentication/Stores/AuthStore";

export const TaskCard: FunctionComponent<{
  id: String;
  parentSession: String;
  title: String;
  url?: String;
  description?: String;
  status: Status;
  complexityAverage?: Number;
  finalValue?: Number;
}> = ({ id, parentSession, title, url, description, status, complexityAverage, finalValue }) => {
  const { isAdmin, userId, token } = useAuthStore();

  const requestStatusChange = async (newStatus: Status) => {
    const url = baseUrl + serviceUrl + parentSession + "/task/status";

    await axios({
      method: "put",
      url: url,
      data: { id: id, status: convertStatusToNumber(newStatus) },
      headers: {
        Accept: "application/json",
        "Content-Type": " application/json",
        Authorization: `Bearer ${token}`,
      },
    });
  };

  console.log("Average: " + complexityAverage);
  console.log("FinalValue: " + finalValue);

  const { deleteTask } = useTaskStore();
  const requestDeleteTask = async () => {
    const url = baseUrl + serviceUrl + parentSession + "/task/" + id;
    const result = await axios.delete(url);

    if (result.status == 200) {
      deleteTask(id);
    }
  };

  const renderCheckmark = () => {
    
      return (  
      <Checkbox
      key = {"checkbox"}  
      size = {"25px"}
      backgroundColor = {"#0DA65A"}
      accentColor = {" #FFFFFF"}
     />
      )

  }

  const renderAdministrativeView = () => {
    return (
      <>
        {(() => {
          switch (status) {
            case Status.Open:
              return (
                <button
                  className={
                    "py-2 px-4 rounded " + convertStatusToColor(status)
                  }
                  onClick={() => requestStatusChange(Status.Evaluated)}
                >
                  Evaluate!
                </button>
              );
            case Status.Evaluated:
              return (
                <button
                  className={
                    "py-2 px-4 rounded " + convertStatusToColor(status)
                  }
                  onClick={() => requestStatusChange(Status.Ended)}
                >
                  Close!
                </button>
              );
            case Status.Suspended:
              return (
                <button
                  className={
                    "py-2 px-4 rounded " + convertStatusToColor(status)
                  }
                  onClick={() => requestStatusChange(Status.Open)}
                >
                  Vote now
                </button>
              );
            case Status.Ended:
              return (
              <>
                <>
                {renderFinalValueOnClosedTasks()}
                </>
                <button
                  className={
                    "bg-orange-500 p-2 mb-2 bg-warning font-bold p-1 mt-2 rounded"
                    }
                    onClick={() => requestStatusChange(Status.Open)}
                >
                  Vote again
                </button>
              </>
              );
            default:
              return <></>
          }
        })()}
        <button
          onClick={requestDeleteTask}
          className={
            "bg-red-500 hover:bg-red-700 text-white font-bold p-1 mt-2 rounded"
          }
        >
          Delete
        </button>
      </>
    );
  };

  const colorStyle = {
    color: '#253EEA',
    fontWeight: 'bold',
  };

  /*
  function UseUpdateFinalValue() {
  const [value, setValue] = useState(0);
  return () => setValue(value => value + 1)
  }

  const forceUpdate = UseUpdateFinalValue()
  */

  const renderFinalValueOnClosedTasks = () => (
    <>
      <>
        {renderCheckmark()}
      </>
        <div style={colorStyle}>
          Rated:  {finalValue}
        </div>

    </>
  );

  return (
    <>
      <div
        className={
          "flex flex-col border-l-4 m-2 p-4 bg-white rounded " +
          convertStatusToBorderColor(status)
        }
      >
        {isAdmin() ? renderAdministrativeView() : <></>}
        <div className="flex flex-row justify-between py-2">
          <strong className={convertStatusToTextColor(status)}>{title}</strong>
          <p className={convertStatusToTextColor(status)}>{url}</p>
        </div>
        <hr />
        <p className="pt-2">{description}</p>
      </div>
    </>
  );
};
