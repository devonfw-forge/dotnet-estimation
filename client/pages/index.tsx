import useWebSocket from "react-use-websocket";
import { EstimationBar } from "./components/features/Session/Estimation/Components/EstimationBar";
import { StickyHeader } from "./components/Globals/StickyHeader";
import { dummyTasks, dummyUsers } from "./components/Globals/DummyData";
import { useEffect } from "react";
import { Frame } from "./components/Globals/Frame";
import { App } from "./components/Globals/App";
import { UserView } from "./components/features/Session/Users/Components/UserView";
import { Estimation } from "./components/features/Session/Estimation/Components/Estimation";
import { TaskView } from "./components/features/Session/Tasks/Components/TaskView";
import { useTaskStore } from "./components/features/Session/Tasks/Stores/TaskStore";
import { useSessionUserStore } from "./components/features/Session/Users/Stores/UserStore";
import { IncomingMessage } from "./Types/MessageTypes";
import { Action } from "./Types/Action";
import { IWebSocketMessage } from "./Interfaces/IWebSocketMessage";
import { OnConnectMessage } from "./Interfaces/OnConnectMessage";

export default function Home() {
  const { setCurrentUsers } = useSessionUserStore();
  const { setCurrentTasks } = useTaskStore();

  useEffect(() => {
    setCurrentUsers(dummyUsers);
    setCurrentTasks(dummyTasks);
  }, []);

  const submitAnswerToRestAPI = async () => {
    // const { complexity, effort, risk } = extractValues();
    // TODO: submit to rest
    // remove task from current store // actually dont!!! Just make it not active... keep it for the history?
  };

  //  process onUserConnect, onAnotherUserConnect, markTaskAsActive,
  const processMessage = (message: IWebSocketMessage) => {
    let { type, data } = message;

    let { msg } = JSON.parse(data);

    let parsedMessage: IncomingMessage = msg;

    switch (parsedMessage.action) {
      case Action.StartedTaskEstimation:
        console.log(parsedMessage);

        let { id } = parsedMessage;

        // mark task as active if the task already exists

        // else crate entry for task and mark as active

        // need to reset the value stored
        // resetStore();

        break;
      case Action.StoppedTaskEstimation: {
      }
    }
  };

  const { sendMessage, getWebSocket } = useWebSocket(
    "ws://localhost:8080/lobby/a",
    {
      onOpen: (event: any) => {
        let { type, data } = event;

        let { msg } = JSON.parse(data);

        let parsedMessage: OnConnectMessage = msg;

        let { users, tasks } = parsedMessage;

        setCurrentTasks(tasks);
        setCurrentUsers(users);
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
