import axios from "axios";
import { useRouter } from "next/router";
import { useEffect, useImperativeHandle } from "react";
import useWebSocket from "react-use-websocket";
import { Estimation } from "../../app/Components/Features/Session/Estimation/Components/Estimation";
import { useEstimationStore } from "../../app/Components/Features/Session/Estimation/Stores/EstimationStore";
import { TaskView } from "../../app/Components/Features/Session/Tasks/Components/TaskView";
import { useTaskStore } from "../../app/Components/Features/Session/Tasks/Stores/TaskStore";
import { UserView } from "../../app/Components/Features/Session/Users/Components/UserView";
import { useSessionUserStore } from "../../app/Components/Features/Session/Users/Stores/UserStore";
import { App } from "../../app/Components/Globals/App";
import { Frame } from "../../app/Components/Globals/Frame";
import { StickyHeader } from "../../app/Components/Globals/StickyHeader";
import { baseUrl, serviceUrl } from "../../app/Constants/url";
import { IMessage, ITypedMessage } from "../../app/Interfaces/IMessage";
import { ITask, ITaskStatusChange } from "../../app/Interfaces/ITask";
import { IWebSocketMessage } from "../../app/Interfaces/IWebSocketMessage";
import { Type } from "../../app/Types/Type";
import { dummyUsers } from "../../app/Components/Globals/DummyData";
import { IEstimationDto } from "../../app/Interfaces/IEstimationDto";
import { useAuthStore } from "../../app/Components/Features/Authentication/Stores/AuthStore";

import jwt, { JwtPayload } from "jsonwebtoken";
import { Role, toRole } from "../../app/Types/Role";
import { UserDto } from "../../app/Types/UserDto";
import { IUser } from "../../app/Interfaces/IUser";

export default function Session({ id, tasks, users, auth, inviteToken }: any) {
  const { setCurrentTasks } = useTaskStore();
  const { setCurrentUsers } = useSessionUserStore();
  const { login, username } = useAuthStore();
  const { addUser } = useSessionUserStore();

  useEffect(() => {
    setCurrentTasks(tasks);
    setCurrentUsers(users);

    const { role, username, userId, token } = auth;

    login(username, token, userId, role);
  }, [auth, tasks, users]);

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
      case Type.EstimationAdded: {
        let { payload } = parsed as IMessage<IEstimationDto>;

        // add estimation
        console.log(payload);
        upsertEstimationToTask(payload);

        break;
      }
      case Type.UserJoined: {
        let { payload } = parsed as IMessage<IUser>;
        console.log(payload);
        addUser(payload);
      }
      default: {
        break;
      }
    }
  };

  const { upsertTask, changeStatusOfTask, deleteTask, upsertEstimationToTask } =
    useTaskStore();

  const { sendMessage, getWebSocket } = useWebSocket(
    "ws://localhost:8085/" + id + "/ws/" + auth.token,
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
        <StickyHeader inviteToken={inviteToken} />
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
  const { params, query } = context;

  const { id } = params;

  const { token } = query;

  const {
    role,
    unique_name: username,
    nameid: userId,
  } = jwt.decode(token) as JwtPayload;

  const { data, status } = await axios({
    method: "get",
    url: "http://127.0.0.1:8085/estimation/v1/session/" + id + "/status",
    headers: {
      Accept: "application/json",
      "Content-Type": " application/json",
      Authorization: `Bearer ${token}`,
    },
  });

  const { inviteToken, tasks, users } = JSON.parse(data);

  const auth = { userId, token, role: toRole(role), username };

  return {
    props: { id, tasks, users, auth, inviteToken }, // will be passed to the page component as props
  };
}
