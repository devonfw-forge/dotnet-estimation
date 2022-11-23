import axios from "axios";
import { useRouter } from "next/router";
import { useEffect } from "react";
import useWebSocket from "react-use-websocket";
import { baseUrl, serviceUrl } from "../../../Constants/url";
import { ITypedMessage, IMessage } from "../../../Interfaces/IMessage";
import { ITask, ITaskStatusChange } from "../../../Interfaces/ITask";
import { IWebSocketMessage } from "../../../Interfaces/IWebSocketMessage";
import { Type } from "../../../Types/Type";
import { App } from "../../Globals/App";
import { Frame } from "../../Globals/Frame";
import { StickyHeader } from "../../Globals/StickyHeader";
import { Estimation } from "./Estimation/Components/Estimation";
import { TaskView } from "./Tasks/Components/TaskView";
import { useTaskStore } from "./Tasks/Stores/TaskStore";
import { UserView } from "./Users/Components/UserView";

export default function Session({ id }: any) {
  const store = useTaskStore();

  //  process onUserConnect, onAnotherUserConnect, markTaskAsActive,
  const processMessage = (message: IWebSocketMessage) => {
    let { data } = message;

    let parsed = JSON.parse(data);

    // cast incoming message and extract type
    let { type } = parsed as ITypedMessage;

    switch (type) {
      case Type.TaskCreated: {
        // cast incoming message and extract payload
        let { payload } = parsed as IMessage<ITask>;

        upsertTask(payload);
        break;
      }
      case Type.TaskStatusModified: {
        let { payload } = parsed as IMessage<ITaskStatusChange[]>;

        payload.forEach((statusChanges) => {
          changeStatusOfTask(statusChanges.id, statusChanges.status);
        });

        break;
      }
      default: {
        break;
      }
    }
  };

  const { upsertTask, changeStatusOfTask } = useTaskStore();

  const { sendMessage, getWebSocket } = useWebSocket(
    "ws://localhost:8085/" + id + "/ws",
    {
      onOpen: (event: any) => {
        console.log(event);
      },
      onMessage: (event: WebSocketEventMap["message"]) => processMessage(event),
      onClose: (event: any) => console.log("Connection closed"),
      onError: (error: any) => console.log("Error occured"),
    }
  );

  return (
    <>
      <Frame>
        <StickyHeader />
        <App>
          <UserView key={"userView"} />
          <Estimation key={"estimationView"} id={id} />
          <TaskView key={"taskView"} id={id} />
        </App>
      </Frame>
    </>
  );
}
