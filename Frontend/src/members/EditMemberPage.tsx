import { useState, type SubmitEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Link, useNavigate, useParams } from "react-router-dom";
import { ApiError } from "../api";
import { getAccessToken } from "../auth/tokenStorage";
import { getMember, updateMember } from "./membersApi";
import type { UpdateMemberRequest } from "./types";
import { ReadId } from "../RouteUtils";

export function EditMemberPage() {
  const { memberId: memberIdParameter } = useParams();
  const memberId = ReadId(memberIdParameter);
  const [formError, setFormError] = useState<string | null>(null);
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const memberQuery = useQuery({
    queryKey: ["members", "detail", memberId],
    queryFn: () => {
      if (memberId === null) {
        throw new Error("Invalid member id");
      }

      return getMember(memberId);
    },
    enabled: memberId !== null,
    retry: false,
  });

  const updateMutation = useMutation({
    mutationFn: (request: UpdateMemberRequest) => {
      if (memberId === null) {
        throw new Error("Invalid member id");
      }

      return updateMember(memberId, request);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["members"] });
      navigate(`/members/${memberId}`);
    },
  });

  if (!getAccessToken()) {
    return (
      <main>
        <p>You need to be logged in to do this.</p>
        <Link to="/login">Log in</Link>
      </main>
    );
  }

  if (memberId === null) {
    return (
      <main>
        <h1>Invalid member id</h1>
        <Link to="/members">Back to members</Link>
      </main>
    );
  }

  if (memberQuery.isPending) {
    return <p>Loading member...</p>;
  }

  const queryNotFound =
    memberQuery.error instanceof ApiError && memberQuery.error.status === 404;

  if (queryNotFound) {
    return (
      <main>
        <h1>Member not found</h1>
        <Link to="/members">Back to members</Link>
      </main>
    );
  }

  if (memberQuery.isError) {
    return <p>Could not load the member.</p>;
  }

  const member = memberQuery.data;
  const mutationStatus =
    updateMutation.error instanceof ApiError
      ? updateMutation.error.status
      : null;

  return (
    <main>
      <Link to={`/members/${member.id}`}>Cancel</Link>
      <h1>Edit {member.name}</h1>

      <form onSubmit={handleSubmit}>
        <label>
          Name
          <input
            name="name"
            defaultValue={member.name}
            maxLength={100}
            required
          />
        </label>

        <label>
          Email
          <input
            name="email"
            type="email"
            defaultValue={member.email}
            maxLength={200}
            required
          />
        </label>

        <button type="submit" disabled={updateMutation.isPending}>
          {updateMutation.isPending ? "Saving..." : "Save changes"}
        </button>
      </form>

      {formError && <p>{formError}</p>}
      {mutationStatus === 400 && <p>The API rejected the member data.</p>}
      {mutationStatus === 401 && <p>Your login is missing or expired.</p>}
      {mutationStatus === 403 && (
        <p>
          You can only edit your own account, unless you are an administrator.
        </p>
      )}
      {mutationStatus === 404 && <p>This member no longer exists.</p>}
      {mutationStatus === 409 && (
        <p>This email address is already in use by another member.</p>
      )}
      {updateMutation.isError && mutationStatus === null && (
        <p>Could not update the member.</p>
      )}
    </main>
  );

  function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);

    const formData = new FormData(event.currentTarget);
    const name = formData.get("name")?.toString().trim() ?? "";
    const email = formData.get("email")?.toString().trim() ?? "";

    if (!name || !email) {
      setFormError("Enter a name and email.");
      return;
    }

    updateMutation.mutate({ name, email });
  }
}
