import { FunctionComponent, ReactNode } from "react";

export const App: FunctionComponent<{ children: ReactNode }> = ({
  children,
}) => {
  return (
    <>
      <div className="flex flex-row justify-between w-full h-full items-center">
        {children}
      </div>
    </>
  );
};
