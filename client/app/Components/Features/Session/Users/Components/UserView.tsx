import { FunctionComponent, useState } from "react";
import { UserCard } from "./UserCard";
import { useSessionUserStore } from "../Stores/UserStore";

export const UserView: FunctionComponent<{}> = () => {
  const [showImagesOnly, setShowImagesOnly] = useState<boolean>(true);

  const { users } = useSessionUserStore();

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
          <UserCard
            showImagesOnly={showImagesOnly}
            username={item.username}
            imageUrl={item.imageUrl}
            key={"usercard" + item.id}
          />
        ))}
      </div>
    </>
  );
};
