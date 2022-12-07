import axios from "axios";
import { useRouter } from "next/router";
import { FunctionComponent, useState } from "react";
import { baseUrl, serviceUrl } from "../../../../../Constants/url";
import { Status } from "../../../../../Types/Status";
import { useAuthStore } from "../../../Authentication/Stores/AuthStore";

interface TaskCreationProps {
  id: String;
}

export const TaskCreationForm: FunctionComponent<TaskCreationProps> = ({
  id,
}) => {
  const { userId, token } = useAuthStore();

  const [createNew, setCreateNew] = useState(false);
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState<string | undefined>(undefined);

  const submitToRestApi = async (element: any) => {
    element.preventDefault();

    const task = { title, description, url: null };

    const url = baseUrl + serviceUrl + id + "/task";

    // submit to api
    await axios({
      method: "post",
      url: url,
      data: task,
      headers: {
        Accept: "application/json",
        "Content-Type": " application/json",
        Authorization: `Bearer ${token}`,
      },
    });

    setCreateNew(false);
    setTitle("");
    setDescription(undefined);
  };

  return createNew ? (
    <div>
      <div
        className="shadow"
        style={{
          height: "100vh",
          width: "100vw",
          display: "block",
          position: "fixed",
          zIndex: 9999,
          top: 0,
          left: 0,
          backgroundColor: "rgba(120,120,120, 0.9)",
          backdropFilter: "blur(5px)",
        }}
      >
        <div
          style={{
            position: "relative",
            top: "15%",
            width: "20%",
            textAlign: "center",
            marginTop: "30px",
            margin: "auto",
          }}
          className="bg-gray-200 rounded-2"
        >
          <div style={{ padding: "20px" }}>
            <form onSubmit={submitToRestApi} className="flex flex-col gap-2">
              <label className="text-muted" htmlFor="title">
                Title:
              </label>
              <input
                type="text"
                id="title"
                name="title"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                className="rounded"
              />
              <label className="text-muted" htmlFor="description">
                Description:
              </label>
              <input
                type="text"
                id="description"
                name="description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
              />
              <input
                className={
                  "border-b-blue-700 bg-green-500 hover:bg-green-700 text-white font-bold p-2 rounded "
                }
                type="submit"
                value="Create!"
              />
            </form>
          </div>
        </div>
      </div>
    </div>
  ) : (
    <>
      <div className="flex-shrink flex items-center justify-center p-2">
        <button
          className={
            "border-b-blue-700 bg-green-500 hover:bg-green-700 text-white font-bold p-2 rounded flex-shrink-1"
          }
          onClick={() => setCreateNew(true)}
        >
          Create
        </button>
      </div>
    </>
  );
};
