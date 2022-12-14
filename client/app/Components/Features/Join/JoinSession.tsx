import axios from "axios";
import { useRouter } from "next/router";
import { useState } from "react";
import { baseUrl, serviceUrl } from "../../../Constants/url";
import { Role, toNumber, toRole } from "../../../Types/Role";
import { Frame } from "../../Globals/Frame";

export default function JoinSession() {
  const router = useRouter();
  const [joinToken, setJoinToken] = useState("");
  const [username, setUsername] = useState("");
  const [desiredRole, setDesiredRole] = useState(Role.Spectator);

  const submitTokenAndRedirect = async (element: any) => {
    element.preventDefault();

    const url = `${baseUrl}${serviceUrl}${joinToken}/entry`;

    const { data, status } = await axios({
      method: "post",
      url: url,
      data: { username, role: toNumber(desiredRole) },
    });

    const { token, sessionId } = data;

    if (status === 200) {
      if (token != undefined || token != "") {
        router.push(`/session/${sessionId}?token=${token}`);
      }
    }
  };

  return (
    <>
      <Frame>
        <div className="flex flex-row content-center items-center">
          <form
            onSubmit={submitTokenAndRedirect}
            className="flex flex-col gap-2"
          >
            <label className="text-muted" htmlFor="username">
              Your name:
            </label>
            <input
              type="text"
              id="username"
              name="username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
            <label className="text-muted" htmlFor="title">
              InviteToken:
            </label>
            <input
              type="text"
              id="title"
              name="title"
              value={joinToken}
              onChange={(e) => setJoinToken(e.target.value)}
              className="rounded"
            />{" "}
            <label className="text-muted" htmlFor="role">
              Your anticipated role:
            </label>
            <div>
              <input
                type="radio"
                value="Voter"
                name="role"
                onChange={(e) => setDesiredRole(Role.Voter)}
              />{" "}
              Voter
              <input
                type="radio"
                value="Spectator"
                name="role"
                onChange={(e) => setDesiredRole(Role.Spectator)}
              />{" "}
              Spectator
            </div>
            <input
              className={
                "border-b-blue-700 bg-green-500 hover:bg-green-700 text-white font-bold p-2 rounded "
              }
              type="submit"
              value="Create!"
            />
          </form>
        </div>
      </Frame>
    </>
  );
}
