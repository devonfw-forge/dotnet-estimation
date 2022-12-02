import { FunctionComponent, useState } from "react";
import { UserCard } from "./UserCard";
import { useSessionUserStore } from "../Stores/UserStore";
import { useTaskStore } from "../../Tasks/Stores/TaskStore";
import { Status } from "../../../../../Types/Status";

export const UserView: FunctionComponent<{}> = () => {
  const [showImagesOnly, setShowImagesOnly] = useState<boolean>(true);

  const { users } = useSessionUserStore();

  const { tasks, userAlreadyVoted } = useTaskStore();
  const task = tasks.find((item) => item.status == Status.Open);

  return (
    <>
      <div className="flex flex-col max-w-[20%] h-full overflow-y-auto align-center bg-gray-50">
        {showImagesOnly ? (
          <div className="flex flex-row justify-start">
            <button
              style={{
                rotate: "90deg",
              }}
              className="to-blue-600 p-4"
              onClick={() => setShowImagesOnly(!showImagesOnly)}
            >
              =
            </button>
          </div>
        ) : (
          <div className="flex flex-row justify-around align-center items-center">
            <button
              className="to-blue-600"
              onClick={() => setShowImagesOnly(!showImagesOnly)}
            >
              =
            </button>
            <strong>Users</strong>
          </div>
        )}
        {users.map((item, index) => (
          <>
            {task ? (
              <UserCard
                showImagesOnly={showImagesOnly}
                username={item.username}
                imageUrl={item.imageUrl}
                alreadyVotedForCurrentTask={userAlreadyVoted(item.id, task.id)}
                key={"usercard" + item.id}
              />
            ) : (
              <UserCard
                showImagesOnly={showImagesOnly}
                username={item.username}
                imageUrl={item.imageUrl}
                alreadyVotedForCurrentTask={false}
                key={"usercard" + item.id}
              />
            )}
          </>
        ))}
      </div>
    </>
  );
};
