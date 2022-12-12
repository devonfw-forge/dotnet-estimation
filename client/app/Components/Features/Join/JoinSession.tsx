import axios from "axios";
import { useRouter } from "next/router";
import { useState } from "react";
import { baseUrl, serviceUrl } from "../../../Constants/url";
import { Frame } from "../../Globals/Frame";

export default function JoinSession() {
  const router = useRouter();
  const [sessionId, setSessionId] = useState("");
  const [username, setUsername] = useState("");

  const submitTokenAndRedirect = async (element: any) => {
    element.preventDefault();

    const url = `${baseUrl}${serviceUrl}${sessionId}/entry`;

    const { data, status } = await axios({
      method: "post",
      url: url,
      data: { username },
    });

    const { token } = data;

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
            <input
              type="text"
              id="username"
              name="username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
            <label className="text-muted" htmlFor="title">
              Title:
            </label>
            <input
              type="text"
              id="title"
              name="title"
              value={sessionId}
              onChange={(e) => setSessionId(e.target.value)}
              className="rounded"
            />{" "}
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
