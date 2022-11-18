import { FunctionComponent, ReactNode } from "react";

// provides a container on the full screen of the user
export const Frame: FunctionComponent<{ children: ReactNode }> = ({
  children,
}) => {
  return (
    <>
      <div className="flex flex-col h-screen w-screen bg-gray-200">
        {children}
      </div>
    </>
  );
};
