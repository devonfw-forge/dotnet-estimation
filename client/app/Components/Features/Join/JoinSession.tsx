import { useRouter } from "next/router";
import { useState } from "react";
import { Frame } from "../../Globals/Frame";

export default function JoinSession() {
  const router = useRouter();
  const [token, setToken] = useState("");

  const submitTokenAndRedirect = (element: any) => {
    element.preventDefault();

    if (token != undefined || token != "") {
      router.push("/session/" + token);
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
              type="text"
              id="title"
              name="title"
              value={token}
              onChange={(e) => setToken(e.target.value)}
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
