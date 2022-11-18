import axios from "axios";
import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import { baseUrl, serviceUrl } from "../../../Constants/url";
import { Frame } from "../../Globals/Frame";

export default function JoinSession() {
  const router = useRouter();

  const [expiresAt, setExpiresAt] = useState(
    new Date(new Date().toString().split("GMT")[0] + " UTC")
      .toISOString()
      .split(".")[0]
  );

  const submitTokenAndRedirect = async (element: any) => {
    element.preventDefault();

    const url = baseUrl + serviceUrl + "/session";

    // if able to create a new session
    //     redirect to the matching websocket
    const result = await axios({
      method: "post",
      url: url,
      data: { ExpiresAt: expiresAt },
    });

    if (result.status == 200) {
      router.push("/session/" + result.data.id);
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
            <label className="text-muted" htmlFor="title">
              Title:
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
