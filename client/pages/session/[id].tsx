import { useRouter } from "next/router";
import useWebSocket from "react-use-websocket";
import { Estimation } from "../../app/Components/Features/Session/Estimation/Components/Estimation";
import { TaskView } from "../../app/Components/Features/Session/Tasks/Components/TaskView";
import { useTaskStore } from "../../app/Components/Features/Session/Tasks/Stores/TaskStore";
import { UserView } from "../../app/Components/Features/Session/Users/Components/UserView";
import { App } from "../../app/Components/Globals/App";
import { Frame } from "../../app/Components/Globals/Frame";
import { StickyHeader } from "../../app/Components/Globals/StickyHeader";
import { IMessage, ITypedMessage } from "../../app/Interfaces/IMessage";
import { ITask } from "../../app/Interfaces/ITask";
import { IWebSocketMessage } from "../../app/Interfaces/IWebSocketMessage";
import { Type } from "../../app/Types/Type";

export default function Home() {
  const router = useRouter();
  const { upsertTask } = useTaskStore();

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
      }
      default: {
        break;
      }
    }
  };

  const { id } = router.query;

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
          <Estimation key={"estimationView"} />
          <TaskView key={"taskView"} />
        </App>
      </Frame>
    </>
  );
}
