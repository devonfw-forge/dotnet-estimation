import { FunctionComponent } from "react";

export const UserCard: FunctionComponent<{
  showImagesOnly: boolean;
  username: String;
  imageUrl?: String;
}> = ({ showImagesOnly, username, imageUrl }) => {
  return (
    <>
      <div className="border-l-4 border-l-blue-400 m-2 p-2">
        {showImagesOnly == true ? (
          <p>Icon</p>
        ) : (
          <div className="flex justify-between">
            <p className="truncate">{username}</p>
            <p>Icon</p>
          </div>
        )}
      </div>
    </>
  );
};
