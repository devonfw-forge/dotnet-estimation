import axios from "axios";
import { FunctionComponent } from "react";
import {
  Status,
  convertStatusToNumber,
  convertStatusToColor,
  convertStatusToBorderColor,
  convertStatusToTextColor,
} from "../../../../../Types/Status";
import { useTaskStore } from "../Stores/TaskStore";
import { baseUrl, serviceUrl } from "../../../../../Constants/url";
import { useAuthStore } from "../../../Authentication/Stores/AuthStore";

export const TaskCard: FunctionComponent<{
  id: String;
  parentSession: String;
  title: String;
  url?: String;
  description?: String;
  status: Status;
}> = ({ id, parentSession, title, url, description, status }) => {
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

  const { deleteTask } = useTaskStore();
  const requestDeleteTask = async () => {
    const url = baseUrl + serviceUrl + parentSession + "/task/" + id;
    const result = await axios.delete(url);

    if (result.status == 200) {
      deleteTask(id);
    }
  };

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
                <button
                  className={
                    "bg-orange-500 p-2 mb-2 bg-warning font-bold p-1 mt-2 rounded"
                    }
                    onClick={() => requestStatusChange(Status.Open)}
                >

                  Vote again
                </button>
              );
            default:
              return <></>;
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
