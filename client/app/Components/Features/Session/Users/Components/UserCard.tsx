import { FunctionComponent } from "react";

export const UserCard: FunctionComponent<{
  showImagesOnly: boolean;
  username: String;
  imageUrl?: String;
  alreadyVotedForCurrentTask: boolean;
}> = ({ showImagesOnly, username, imageUrl, alreadyVotedForCurrentTask }) => {
  return (
    <>
      <div className="border-l-4 border-l-blue-400 m-2 p-2 flex justify-between items-center w-36">
        {showImagesOnly == true ? (
          <>
            <p>Icon</p>
            {alreadyVotedForCurrentTask ? (
              <div className="rounded-full bg-blue-600 w-4 h-4"></div>
            ) : (
              <></>
            )}
          </>
        ) : (
          <>
            <p className="truncate">{username}</p>
            <p>Icon</p>
            {alreadyVotedForCurrentTask ? (
              <div className="rounded-full bg-blue-600 w-4 h-4"></div>
            ) : (
              <></>
            )}
          </>
        )}
      </div>
    </>
  );
};
