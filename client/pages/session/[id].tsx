import axios from "axios";
import { useRouter } from "next/router";
import { useEffect } from "react";
import useWebSocket from "react-use-websocket";
import { Estimation } from "../../app/Components/Features/Session/Estimation/Components/Estimation";
import { TaskView } from "../../app/Components/Features/Session/Tasks/Components/TaskView";
import { useTaskStore } from "../../app/Components/Features/Session/Tasks/Stores/TaskStore";
import { UserView } from "../../app/Components/Features/Session/Users/Components/UserView";
import { App } from "../../app/Components/Globals/App";
import { Frame } from "../../app/Components/Globals/Frame";
import { StickyHeader } from "../../app/Components/Globals/StickyHeader";
import { baseUrl, serviceUrl } from "../../app/Constants/url";
import { IMessage, ITypedMessage } from "../../app/Interfaces/IMessage";
import { ITask, ITaskStatusChange } from "../../app/Interfaces/ITask";
import { IWebSocketMessage } from "../../app/Interfaces/IWebSocketMessage";
import { Type } from "../../app/Types/Type";

export default function Session({ id, data }: any) {
  const { setCurrentTasks } = useTaskStore();

  useEffect(() => {
    setCurrentTasks(data.tasks);
  }, [data]);

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
      case Type.TaskDeleted: {
        let { payload } = parsed as IMessage<String>;

        deleteTask(payload);
        break;
      }
      default: {
        break;
      }
    }
  };

  const { upsertTask, changeStatusOfTask, deleteTask } = useTaskStore();

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

export async function getServerSideProps(context: any) {
  const { params } = context;

  const { id } = params;

  const res = await fetch(
    "http://127.0.0.1:8085/estimation/v1/session/" + id + "/status"
  );

  const data = await res.json();

  return {
    props: { id, data }, // will be passed to the page component as props
  };
}
