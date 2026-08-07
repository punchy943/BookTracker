import { apiRequest, apiRequestWithoutResponse } from "../api";
import { 
  type RegisterMemberRequest, 
  type RegisterMemberResponse, 
  type MemberSummary, 
  type GetMemberSummariesRequest, 
  type MemberDetails,
  type UpdateMemberRequest
} from "./types";
import type { PagedResult } from "../types";

export function registerMember(request: RegisterMemberRequest) {
  return apiRequest<RegisterMemberResponse>("/members", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export function getMembers(request: GetMemberSummariesRequest) {
  const parameters = new URLSearchParams({
    page: request.page.toString(),
    pageSize: request.pageSize.toString(),
  });

  if (request.search) {
    parameters.set("search", request.search);
  }

  return apiRequest<PagedResult<MemberSummary>>(
    `/members?${parameters.toString()}`,
  )
}

export function getMember(memberId: number) {
  return apiRequest<MemberDetails>(`/members/${memberId}`);
}

export function updateMember(memberId: number, request: UpdateMemberRequest) {
  return apiRequestWithoutResponse(`/members/${memberId}`, {
    method: "PUT",
    body: JSON.stringify(request)
  });
}