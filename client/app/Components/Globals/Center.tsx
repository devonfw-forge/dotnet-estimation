import { FunctionComponent, ReactNode } from "react";

export const Center: FunctionComponent<{ children: ReactNode }> = ({
  children,
}) => {
  return (
    <>
      <div className="flex flex-col bg-white rounded">
        <div className={"flex flex-col items-center"}></div>
        {children}
      </div>
    </>
  );
};
