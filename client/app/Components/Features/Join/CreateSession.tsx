import axios from "axios";
import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import { baseUrl, serviceUrl } from "../../../Constants/url";
import { Frame } from "../../Globals/Frame";

export default function CreateSession() {
  const router = useRouter();

  const [expiresAt, setExpiresAt] = useState(
    new Date(new Date().toString().split("GMT")[0] + " UTC")
      .toISOString()
      .split(".")[0]
  );

  const [username, setUsername] = useState("");

  const submitAndRedirect = async (element: any) => {
    element.preventDefault();

    const url = baseUrl + serviceUrl + "newSession";

    const { data, status } = await axios({
      method: "post",
      url: url,
      data: { ExpiresAt: expiresAt, username },
    });

    const { id, token } = data;

    console.log(id, token);

    if (status == 200) {
      router.push(`/session/${id}?token=${token}`);
    }
  };

  return (
    <>
      <Frame>
        <div className="flex flex-row content-center items-center">
          <form onSubmit={submitAndRedirect} className="flex flex-col gap-2">
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
            <label className="text-muted" htmlFor="expiresAt">
              Expire at:
            </label>
            <input
              type="datetime-local"
              id="expiresAt"
              name="expiresAt"
              value={expiresAt.toString()}
              onChange={(e) =>
                setExpiresAt(
                  new Date(
                    new Date(e.target.value).toString().split("GMT")[0] + " UTC"
                  )
                    .toISOString()
                    .split(".")[0]
                )
              }
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
