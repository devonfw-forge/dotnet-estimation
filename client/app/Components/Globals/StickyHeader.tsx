import { FunctionComponent } from "react";

export const StickyHeader: FunctionComponent<{}> = () => {
  return (
    <>
      <header
        className="flex flex-row items-center justify-around border-b-2 shadow-md w-full sticky t-0 bg-gradient-to-r from-purple-600 to-blue-600 p-4 text-white"
        style={{ zIndex: 99999 }}
      >
        <a className="nav-link px-2 link-primary">Capgemini-Logo</a>
        <div className="d-flex flex-row">
          <a className="nav-link px-2 link-secondary">Estimation</a>
          <a className="nav-link px-2 link-secondary">User-Image</a>
        </div>
      </header>
    </>
  );
};
