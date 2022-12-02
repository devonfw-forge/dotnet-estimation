import { FunctionComponent, useState } from "react";
import { useAuthStore } from "../Stores/AuthStore";

export const LoginForm: FunctionComponent<{}> = ({}) => {
  const { login } = useAuthStore();

  const [username, setUserName] = useState<string>("");

  return (
    <>
      <form
        onSubmit={(e) => {
          e.preventDefault();
          login(username);
        }}
      >
        <input
          id="loginUsername"
          type="text"
          name="username"
          value={username}
          onChange={(e) => setUserName(e.target.value)}
        />
        <input type="submit" value="Submit" />
      </form>
    </>
  );
};
